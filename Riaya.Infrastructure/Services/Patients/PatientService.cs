using Microsoft.EntityFrameworkCore;
using Riaya.Application.Features.Patients.DTOs;
using Riaya.Application.Features.Patients.Interfaces;
using Riaya.Domain.Exceptions;
using Riaya.Persistence.Context;

namespace Riaya.Infrastructure.Services.Patients;

public class PatientService : IPatientService
{
    private readonly AppDbContext _context;

    public PatientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PatientProfileDto> GetProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("Patient not found");

        return new PatientProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }

    public async Task UpdateProfileAsync(Guid userId, UpdatePatientProfileRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("Patient not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        await _context.SaveChangesAsync();
    }
}