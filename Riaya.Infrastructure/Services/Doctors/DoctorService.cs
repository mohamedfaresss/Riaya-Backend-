using Microsoft.EntityFrameworkCore;
using Riaya.Application.Features.Doctors.DTOs;
using Riaya.Application.Features.Doctors.Interfaces;
using Riaya.Application.Features.TimeSlots.DTOs;
using Riaya.Domain.Entities;
using Riaya.Domain.Exceptions;
using Riaya.Persistence.Context;

namespace Riaya.Infrastructure.Services.Doctors;

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _context;

    public DoctorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DoctorProfileDto> GetProfileAsync(Guid userId)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        return new DoctorProfileDto
        {
            Id = doctor.Id,
            FirstName = doctor.User.FirstName,
            LastName = doctor.User.LastName,
            Email = doctor.User.Email,
            Specialty = doctor.Specialty,
            University = doctor.University,
            Experience = MapDoctorExperience(doctor),
            ImageUrl = doctor.ProfileImageUrl
        };
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateDoctorProfileRequest request)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        doctor.User.FirstName = request.FirstName;
        doctor.User.LastName = request.LastName;

        doctor.Specialty = request.Specialty;
        doctor.University = request.University;
        doctor.YearsOfExperience = request.YearsOfExperience;

        if (request.Image != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            doctor.ProfileImageUrl = $"/uploads/{fileName}";
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<DoctorListItemDto>> GetAllDoctorsAsync()
    {
        var now = DateTime.UtcNow;

        return await _context.Doctors
            .Include(d => d.User)
            .Select(d => new DoctorListItemDto
            {
                Id = d.Id,
                FullName = d.User.FirstName + " " + d.User.LastName,
                Email = d.User.Email,
                Specialization = d.Specialty,
                University = d.University,
                YearsOfExperience = d.YearsOfExperience,
                IsAvailable = d.TimeSlots.Any(ts => ts.Booking == null && ts.StartAtUtc > now),
                ProfileImageUrl = d.ProfileImageUrl
            })
            .ToListAsync();
    }

    public async Task<List<TimeSlotDto>> GetMyTimeSlotsAsync(Guid userId)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        return await _context.TimeSlots
            .Where(t => t.DoctorId == doctor.Id)
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

    public async Task DeleteTimeSlotAsync(Guid userId, Guid slotId)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (doctor == null)
            throw new NotFoundException("Doctor not found");

        var slot = await _context.TimeSlots
            .Include(t => t.Booking)
            .FirstOrDefaultAsync(t => t.Id == slotId && t.DoctorId == doctor.Id);

        if (slot == null)
            throw new NotFoundException("TimeSlot not found");

        if (slot.Booking != null)
            throw new ConflictException("Cannot delete a booked slot");

        _context.TimeSlots.Remove(slot);
        await _context.SaveChangesAsync();
    }

    public Task<List<SpecializationDto>> GetSpecializationsAsync()
    {
        var specs = new List<SpecializationDto>
    {
        new() { Value = "All", NameEn = "All", NameAr = "الكل" },
        new() { Value = "Cardiology", NameEn = "Cardiology", NameAr = "أمراض القلب" },
        new() { Value = "Dermatology", NameEn = "Dermatology", NameAr = "الجلدية" },
        new() { Value = "Dentistry", NameEn = "Dentistry", NameAr = "الأسنان" },
        new() { Value = "Neurology", NameEn = "Neurology", NameAr = "الأعصاب" },
        new() { Value = "Pediatrics", NameEn = "Pediatrics", NameAr = "الأطفال" },
        new() { Value = "Orthopedics", NameEn = "Orthopedics", NameAr = "العظام" },
        new() { Value = "ENT", NameEn = "ENT", NameAr = "أنف وأذن وحنجرة" },
        new() { Value = "Ophthalmology", NameEn = "Ophthalmology", NameAr = "العيون" },
        new() { Value = "Psychiatry", NameEn = "Psychiatry", NameAr = "الطب النفسي" }
    };

        return Task.FromResult(specs);
    }

    private static string? MapDoctorExperience(Doctor doctor)
    {
        return doctor.YearsOfExperience.ToString();
    }
}
