using Riaya.Domain.Enums;

namespace Riaya.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid ClinicId { get; set; }

    public string Reason { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public string PaymobOrderId { get; set; } = string.Empty;
    public string PaymobPaymentKey { get; set; } = string.Empty;

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public Patient Patient { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;
    public Clinic Clinic { get; set; } = null!;

    public Booking? Booking { get; set; }
    public Guid? BookingId { get; set; }  // nullable لحد ما الدفع يتأكد

}
