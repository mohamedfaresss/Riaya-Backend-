using Microsoft.EntityFrameworkCore;
using Riaya.Application.Features.TimeSlots.DTOs;
using Riaya.Application.Features.TimeSlots.Interfaces;
using Riaya.Domain.Entities;
using Riaya.Domain.Exceptions;
using Riaya.Persistence.Context;

namespace Riaya.Infrastructure.Services.TimeSlots;

public class TimeSlotService : ITimeSlotService
{
    private readonly AppDbContext _context;

    public TimeSlotService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateTimeSlotAsync(Guid userId, Guid clinicId, CreateTimeSlotRequest request)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId && d.ClinicId == clinicId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        if (request.EndAtUtc <= request.StartAtUtc)
            throw new ConflictException("End time must be after start time");

        var overlapping = await _context.TimeSlots
            .AnyAsync(t =>
                t.DoctorId == doctor.Id &&
                t.ClinicId == clinicId &&
                t.StartAtUtc < request.EndAtUtc &&
                t.EndAtUtc > request.StartAtUtc);

        if (overlapping)
            throw new ConflictException("Doctor already has a slot in this time range");

        var slot = new TimeSlot
        {
            DoctorId = doctor.Id,
            ClinicId = clinicId,
            StartAtUtc = request.StartAtUtc,
            EndAtUtc = request.EndAtUtc
        };

        _context.TimeSlots.Add(slot);
        await _context.SaveChangesAsync();
        return slot.Id;
    }

    public async Task<List<TimeSlotDto>> GetAvailableSlotsAsync(Guid doctorId)
    {
        return await _context.TimeSlots
            .Where(t => t.DoctorId == doctorId)
            .OrderBy(t => t.StartAtUtc)
            .Select(t => new TimeSlotDto
            {
                Id = t.Id,
                StartTime = TimeOnly.FromDateTime(t.StartAtUtc).ToString("HH:mm"),
                EndTime = TimeOnly.FromDateTime(t.EndAtUtc).ToString("HH:mm"),
                Date = DateOnly.FromDateTime(t.StartAtUtc).ToString("yyyy-MM-dd"),
                IsBooked = t.Booking != null
            })
            .ToListAsync();
    }
}
