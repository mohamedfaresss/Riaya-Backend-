using Microsoft.EntityFrameworkCore;
using Riaya.Application.Features.Schedule.DTOs;
using Riaya.Application.Features.Schedule.Interfaces;
using Riaya.Application.Features.TimeSlots.DTOs;
using Riaya.Domain.Entities;
using Riaya.Domain.Exceptions;
using Riaya.Persistence.Context;

namespace Riaya.Infrastructure.Services.Schedule;

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleDto> CreateScheduleAsync(Guid userId, Guid clinicId, CreateScheduleRequest request)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId && d.ClinicId == clinicId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        if (request.EndTime <= request.StartTime)
            throw new ConflictException("End time must be after start time");

        if (request.SlotDurationMinutes < 10 || request.SlotDurationMinutes > 120)
            throw new ConflictException("Slot duration must be between 10 and 120 minutes");

        var exists = await _context.DoctorSchedules
            .AnyAsync(s =>
                s.DoctorId == doctor.Id &&
                s.ClinicId == clinicId &&
                s.DayOfWeek == request.DayOfWeek &&
                s.IsActive &&
                !s.IsDeleted);

        if (exists)
            throw new ConflictException($"Doctor already has a schedule for {request.DayOfWeek}");

        var schedule = new DoctorSchedule
        {
            DoctorId = doctor.Id,
            ClinicId = clinicId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDurationMinutes = request.SlotDurationMinutes
        };

        _context.DoctorSchedules.Add(schedule);
        await _context.SaveChangesAsync();

        return MapToDto(schedule);
    }

    public async Task<List<ScheduleDto>> GetMySchedulesAsync(Guid userId)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        return await _context.DoctorSchedules
            .Where(s => s.DoctorId == doctor.Id && !s.IsDeleted)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task DeleteScheduleAsync(Guid userId, Guid scheduleId)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        var schedule = await _context.DoctorSchedules
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.DoctorId == doctor.Id && !s.IsDeleted);

        if (schedule == null)
            throw new NotFoundException("Schedule not found");

        schedule.IsActive = false;
        schedule.IsDeleted = true;
        schedule.DeletedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<TimeSlotDto>> GenerateSlotsAsync(Guid userId, Guid clinicId, GenerateSlotsRequest request)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId && d.ClinicId == clinicId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        if (request.ToDate < request.FromDate)
            throw new ConflictException("ToDate must be after FromDate");

        if (request.ToDate.DayNumber - request.FromDate.DayNumber > 30)
            throw new ConflictException("Cannot generate slots for more than 30 days");

        var schedules = await _context.DoctorSchedules
            .Where(s => s.DoctorId == doctor.Id && s.ClinicId == clinicId && s.IsActive && !s.IsDeleted)
            .ToListAsync();

        if (!schedules.Any())
            throw new NotFoundException("Doctor has no active schedules");

        var generatedSlots = new List<TimeSlot>();
        var current = request.FromDate;

        while (current <= request.ToDate)
        {
            var schedule = schedules.FirstOrDefault(s => s.DayOfWeek == current.DayOfWeek);

            if (schedule != null)
            {
                var slotStart = current.ToDateTime(schedule.StartTime, DateTimeKind.Utc);
                var slotEnd = current.ToDateTime(schedule.EndTime, DateTimeKind.Utc);

                while (slotStart.AddMinutes(schedule.SlotDurationMinutes) <= slotEnd)
                {
                    var end = slotStart.AddMinutes(schedule.SlotDurationMinutes);

                    var alreadyExists = await _context.TimeSlots
                        .AnyAsync(t => t.DoctorId == doctor.Id && t.StartAtUtc == slotStart);

                    if (!alreadyExists)
                    {
                        generatedSlots.Add(new TimeSlot
                        {
                            DoctorId = doctor.Id,
                            ClinicId = clinicId,
                            StartAtUtc = slotStart,
                            EndAtUtc = end
                        });
                    }

                    slotStart = end;
                }
            }

            current = current.AddDays(1);
        }

        _context.TimeSlots.AddRange(generatedSlots);
        await _context.SaveChangesAsync();

        return generatedSlots
            .OrderBy(s => s.StartAtUtc)
            .Select(s => new TimeSlotDto
            {
                Id = s.Id,
                StartTime = TimeOnly.FromDateTime(s.StartAtUtc).ToString("HH:mm"),
                EndTime = TimeOnly.FromDateTime(s.EndAtUtc).ToString("HH:mm"),
                Date = DateOnly.FromDateTime(s.StartAtUtc).ToString("yyyy-MM-dd"),
                IsBooked = false
            })
            .ToList();
    }

    private static ScheduleDto MapToDto(DoctorSchedule s) => new()
    {
        Id = s.Id,
        DayOfWeek = s.DayOfWeek.ToString(),
        StartTime = s.StartTime,
        EndTime = s.EndTime,
        SlotDurationMinutes = s.SlotDurationMinutes,
        IsActive = s.IsActive
    };
}
