using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riaya.Application.Features.Doctors.DTOs;
using Riaya.Application.Features.Doctors.Interfaces;
using System.Security.Claims;

namespace Riaya.API.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllDoctorsAsync());

    [HttpGet("profile")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetProfileAsync(userId));
    }

    [HttpPut("profile")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateDoctorProfileRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.UpdateProfileAsync(userId, request);
        return Ok(new { message = "Profile updated successfully" });
    }

    [HttpGet("my-slots")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> GetMySlots()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetMyTimeSlotsAsync(userId));
    }

    [HttpGet("specializations")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSpecializations()
        => Ok(await _service.GetSpecializationsAsync());

    [HttpDelete("slots/{slotId}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> DeleteSlot(Guid slotId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.DeleteTimeSlotAsync(userId, slotId);
        return Ok(new { message = "Slot deleted successfully" });
    }
}
