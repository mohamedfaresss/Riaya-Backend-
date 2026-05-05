using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riaya.Application.Features.Doctors.Interfaces;
using Riaya.Application.Features.TimeSlots.DTOs;
using Riaya.Application.Features.TimeSlots.Interfaces;

namespace Riaya.API.Controllers;

[ApiController]
[Route("api/timeslots")]
[Route("api/slots")]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _service;
    private readonly IDoctorService _doctorService;

    public TimeSlotsController(ITimeSlotService service, IDoctorService doctorService)
    {
        _service = service;
        _doctorService = doctorService;
    }

    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Create([FromBody] CreateTimeSlotRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var clinicId = Guid.Parse(User.FindFirst("clinicId")!.Value);

        var slotId = await _service.CreateTimeSlotAsync(userId, clinicId, request);
        return Ok(new { id = slotId });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailable([FromQuery] Guid doctorId)
    {
        var slots = await _service.GetAvailableSlotsAsync(doctorId);
        return Ok(slots);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> GetMySlots()
    {
        var callerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _doctorService.GetMyTimeSlotsAsync(callerId);
        return Ok(result);
    }
}
