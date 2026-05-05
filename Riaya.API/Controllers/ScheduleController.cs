using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riaya.Application.Features.Schedule.DTOs;
using Riaya.Application.Features.Schedule.Interfaces;

namespace Riaya.API.Controllers;

[ApiController]
[Route("api/schedules")]
[Authorize(Roles = "Doctor")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _service;

    public ScheduleController(IScheduleService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var clinicId = Guid.Parse(User.FindFirst("clinicId")!.Value);
        return Ok(await _service.CreateScheduleAsync(userId, clinicId, request));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetMySchedulesAsync(userId));
    }

    [HttpDelete("{scheduleId:guid}")]
    public async Task<IActionResult> Delete(Guid scheduleId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.DeleteScheduleAsync(userId, scheduleId);
        return Ok(new { message = "Schedule deleted successfully" });
    }

    [HttpPost("generate-slots")]
    public async Task<IActionResult> GenerateSlots([FromBody] GenerateSlotsRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var clinicId = Guid.Parse(User.FindFirst("clinicId")!.Value);
        var slots = await _service.GenerateSlotsAsync(userId, clinicId, request);
        return Ok(new { generatedCount = slots.Count, slots });
    }
}
