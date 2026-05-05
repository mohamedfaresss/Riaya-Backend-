using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Riaya.Application.Features.Bookings.DTOs;
using Riaya.Application.Features.Payments.DTOs;
using Riaya.Application.Features.Payments.Interfaces;
using Riaya.Domain.Entities;
using Riaya.Domain.Enums;
using Riaya.Domain.Exceptions;
using Riaya.Persistence.Context;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Riaya.Infrastructure.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    private string ApiKey => _config["Paymob:ApiKey"]!;
    private string IntegrationId => _config["Paymob:IntegrationId"]!;
    private string IframeId => _config["Paymob:IframeId"]!;
    private string HmacSecret => _config["Paymob:HmacSecret"]!;

    private const string BaseUrl = "https://accept.paymob.com/api";

    public PaymentService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
        _http = new HttpClient();
    }

    // ═══════════════════════════════════════════════════
    // STEP 1 — Initiate Payment
    // ═══════════════════════════════════════════════════
    public async Task<InitiatePaymentResponse> InitiatePaymentAsync(Guid userId, Guid clinicId, InitiatePaymentRequest request)
    {
        // 1. Get patient
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ClinicId == clinicId)
            ?? throw new NotFoundException("Patient not found");

        // 2. Get slot
        var slot = await _context.TimeSlots
            .Include(t => t.Booking)
            .FirstOrDefaultAsync(t => t.Id == request.TimeSlotId && t.ClinicId == clinicId)
            ?? throw new NotFoundException("TimeSlot not found");

        if (slot.Booking != null)
            throw new ConflictException("This slot is already booked");

        // 3. Get Paymob auth token
        var authToken = await GetAuthTokenAsync();

        // 4. Create Paymob order
        var amountCents = (int)(slot.Price * 100);
        var paymobOrderId = await CreatePaymobOrderAsync(authToken, amountCents, patient.Id);

        // 5. Get payment key
        var paymentKey = await GetPaymentKeyAsync(
            authToken,
            amountCents,
            paymobOrderId,
            patient.User.FirstName,
            patient.User.LastName,
            patient.User.Email
        );

        // 6. Save Payment record
        var payment = new Payment
        {
            PatientId = patient.Id,
            TimeSlotId = slot.Id,
            ClinicId = clinicId,
            Reason = request.Reason,
            Amount = slot.Price,
            PaymobOrderId = paymobOrderId.ToString(),
            PaymobPaymentKey = paymentKey,
            Status = PaymentStatus.Pending
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return new InitiatePaymentResponse
        {
            PaymentKey = paymentKey,
            IframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{IframeId}?payment_token={paymentKey}",
            PaymentId = payment.Id
        };
    }

    // ═══════════════════════════════════════════════════
    // STEP 2 — Handle Paymob Callback (Webhook)
    // ═══════════════════════════════════════════════════
    public async Task<bool> HandleCallbackAsync(PaymobCallbackDto callback)
    {
        if (callback.Type != "TRANSACTION" || callback.Obj == null)
            return false;

        if (!callback.Obj.Success)
            return false;

        // Find payment by Paymob order ID
        var merchantOrderId = callback.Obj.Order?.MerchantOrderId;
        if (string.IsNullOrEmpty(merchantOrderId))
            return false;

        var payment = await _context.Payments
            .Include(p => p.TimeSlot)
                .ThenInclude(t => t.Booking)
            .FirstOrDefaultAsync(p => p.Id == Guid.Parse(merchantOrderId));

        if (payment == null || payment.Status == PaymentStatus.Success)
            return false;

        // Verify slot still available
        if (payment.TimeSlot.Booking != null)
        {
            payment.Status = PaymentStatus.Failed;
            await _context.SaveChangesAsync();
            return false;
        }

        // Create booking
        var booking = new Booking
        {
            PatientId = payment.PatientId,
            DoctorId = payment.TimeSlot.DoctorId,
            TimeSlotId = payment.TimeSlotId,
            ClinicId = payment.ClinicId,
            Reason = payment.Reason,
            Status = BookingStatus.Confirmed
        };

        _context.Bookings.Add(booking);

        // Update payment
        payment.Status = PaymentStatus.Success;
        payment.BookingId = booking.Id;

        await _context.SaveChangesAsync();
        return true;
    }

    // ═══════════════════════════════════════════════════
    // PAYMOB HELPERS
    // ═══════════════════════════════════════════════════

    private async Task<string> GetAuthTokenAsync()
    {
        var response = await _http.PostAsJsonAsync($"{BaseUrl}/auth/tokens", new { api_key = ApiKey });
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("token").GetString()!;
    }

    private async Task<int> CreatePaymobOrderAsync(string authToken, int amountCents, Guid merchantOrderId)
    {
        var body = new
        {
            auth_token = authToken,
            delivery_needed = false,
            amount_cents = amountCents,
            currency = "EGP",
            merchant_order_id = merchantOrderId.ToString(),
            items = Array.Empty<object>()
        };

        var response = await _http.PostAsJsonAsync($"{BaseUrl}/ecommerce/orders", body);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("id").GetInt32();
    }

    private async Task<string> GetPaymentKeyAsync(
        string authToken,
        int amountCents,
        int orderId,
        string firstName,
        string lastName,
        string email)
    {
        var body = new
        {
            auth_token = authToken,
            amount_cents = amountCents,
            expiration = 3600,
            order_id = orderId,
            billing_data = new
            {
                first_name = firstName,
                last_name = lastName,
                email = email,
                phone_number = "NA",
                apartment = "NA",
                floor = "NA",
                street = "NA",
                building = "NA",
                shipping_method = "NA",
                postal_code = "NA",
                city = "NA",
                country = "EG",
                state = "NA"
            },
            currency = "EGP",
            integration_id = int.Parse(IntegrationId)
        };

        var response = await _http.PostAsJsonAsync($"{BaseUrl}/acceptance/payment_keys", body);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("token").GetString()!;
    }

    // ═══════════════════════════════════════════════════
    // HMAC Verification
    // ═══════════════════════════════════════════════════
    public bool VerifyHmac(string receivedHmac, Dictionary<string, string> data)
    {
        var keys = new[]
        {
            "amount_cents", "created_at", "currency", "error_occured",
            "has_parent_transaction", "id", "integration_id", "is_3d_secure",
            "is_auth", "is_capture", "is_refunded", "is_standalone_payment",
            "is_voided", "order", "owner", "pending", "source_data.pan",
            "source_data.sub_type", "source_data.type", "success"
        };

        var message = string.Concat(keys.Select(k => data.TryGetValue(k, out var v) ? v : ""));
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(HmacSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return computed == receivedHmac.ToLower();
    }
}
