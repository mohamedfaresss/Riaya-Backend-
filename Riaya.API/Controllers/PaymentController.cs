using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riaya.Application.Features.Payments.DTOs;
using Riaya.Application.Features.Payments.Interfaces;
using System.Security.Claims;

namespace Riaya.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentController(IPaymentService service)
    {
        _service = service;
    }

    // المريض يبدأ عملية الدفع
    [HttpPost("initiate")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Initiate([FromBody] InitiatePaymentRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var clinicId = Guid.Parse(User.FindFirst("clinicId")!.Value);
        var result = await _service.InitiatePaymentAsync(userId, clinicId, request);
        return Ok(result);
    }

    // Paymob بيبعت callback بعد الدفع
    [HttpPost("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback([FromBody] PaymobCallbackDto callback)
    {
        await _service.HandleCallbackAsync(callback);
        return Ok();
    }
}
