using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riaya.Application.Common;
using Riaya.Application.Features.Admin.Interfaces;

namespace Riaya.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationParams pagination)
        => Ok(await _adminService.GetAllUsersAsync(pagination));

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _adminService.DeleteUserAsync(id);
        return Ok(new { message = "User deleted successfully" });
    }

    [HttpGet("doctors")]
    public async Task<IActionResult> GetDoctors()
        => Ok(await _adminService.GetAllDoctorsAsync());

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings([FromQuery] PaginationParams pagination)
        => Ok(await _adminService.GetAllBookingsAsync(pagination));

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
        => Ok(await _adminService.GetStatsAsync());
}