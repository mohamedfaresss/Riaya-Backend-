using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using Riaya.Application.Auth.Interfaces;
using Riaya.Application.Features.Auth.DTOs;
using Riaya.Domain.Entities;
using Riaya.Domain.Enums;
using Riaya.Domain.Exceptions;
using Riaya.Persistence.Context;

namespace Riaya.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, UserRole role)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            throw new ConflictException("Email already exists");

        var clinic = await _context.Clinics.FirstOrDefaultAsync();
        if (clinic == null)
        {
            clinic = new Clinic { Name = "Main Clinic" };
            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BC.HashPassword(request.Password),
            Role = role,
            ClinicId = clinic.Id
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        if (role == UserRole.Patient)
            _context.Patients.Add(new Patient { UserId = user.Id, ClinicId = clinic.Id });
        else if (role == UserRole.Doctor)
            _context.Doctors.Add(new Doctor { UserId = user.Id, ClinicId = clinic.Id });

        await _context.SaveChangesAsync();

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // ✅ لو اتحذف مينفعش يدخل
        if (user.IsDeleted)
            throw new UnauthorizedAccessException("Account has been deactivated");

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (token == null || token.IsRevoked || token.ExpiresAtUtc < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        // Revoke القديم
        token.IsRevoked = true;
        await _context.SaveChangesAsync();

        // اعمل جديد
        return await GenerateAuthResponseAsync(token.User);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (token == null)
            throw new NotFoundException("Refresh token not found");

        token.IsRevoked = true;
        await _context.SaveChangesAsync();
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var jwtToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddHours(2);

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        });

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = jwtToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };
    }
}