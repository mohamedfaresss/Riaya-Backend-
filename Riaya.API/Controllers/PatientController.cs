using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riaya.Application.Features.Patients.DTOs;
using Riaya.Application.Features.Patients.Interfaces;
using System.Security.Claims;

namespace Riaya.API.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize(Roles = "Patient")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _service;

    public PatientController(IPatientService service)
    {
        _service = service;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetProfileAsync(userId));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.UpdateProfileAsync(userId, request);
        return Ok(new { message = "Profile updated successfully" });
    }
}