
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riaya.Application.Auth.Interfaces;
using Riaya.Application.Features.Auth.DTOs;
using Riaya.Domain.Enums;

namespace Riaya.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register/patient")]
    public async Task<IActionResult> RegisterPatient(RegisterRequest request)
        => Ok(await _authService.RegisterAsync(request, UserRole.Patient));

    [HttpPost("register/doctor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterDoctor(RegisterRequest request)
        => Ok(await _authService.RegisterAsync(request, UserRole.Doctor));

    [HttpPost("register/admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterAdmin(RegisterRequest request)
        => Ok(await _authService.RegisterAsync(request, UserRole.Admin));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
        => Ok(await _authService.LoginAsync(request));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    => Ok(await _authService.RefreshTokenAsync(request.RefreshToken));

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request)
    {
        await _authService.RevokeTokenAsync(request.RefreshToken);
        return Ok(new { message = "Token revoked successfully" });
    }
}
