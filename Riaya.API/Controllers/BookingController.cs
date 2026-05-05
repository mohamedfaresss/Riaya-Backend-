using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riaya.Application.Features.Bookings.DTOs;
using Riaya.Application.Features.Bookings.Interfaces;

namespace Riaya.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingController(IBookingService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var clinicId = Guid.Parse(User.FindFirst("clinicId")!.Value);
        var bookingId = await _service.CreateBookingAsync(userId, clinicId, request);
        return Ok(new { message = "Booking created successfully", bookingId });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var callerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var callerRole = User.FindFirstValue(ClaimTypes.Role)!;
        var result = await _service.GetMyBookingsAsync(callerId, callerRole);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var callerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.CancelBookingAsync(id, callerId);
        return NoContent();
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> UpdateBookingStatus(Guid id, [FromBody] UpdateBookingStatusDto dto)
    {
        var callerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var callerRole = User.FindFirstValue(ClaimTypes.Role)!;
        var result = await _service.UpdateBookingStatusAsync(id, callerId, callerRole, dto);
        return Ok(result);
    }
}
