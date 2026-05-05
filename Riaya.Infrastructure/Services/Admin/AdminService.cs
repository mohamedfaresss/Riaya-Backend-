using Microsoft.EntityFrameworkCore;
using Riaya.Application.Common;
using Riaya.Application.Features.Admin.DTOs;
using Riaya.Application.Features.Admin.Interfaces;
using Riaya.Domain.Enums;
using Riaya.Domain.Exceptions;
using Riaya.Persistence.Context;

namespace Riaya.Infrastructure.Services.Admin;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminUserDto>> GetAllUsersAsync(PaginationParams pagination)
    {
        var query = _context.Users.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role.ToString()
            })
            .ToListAsync();

        return new PagedResult<AdminUserDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }
    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (user.Role == UserRole.Admin)
            throw new ConflictException("Cannot delete an Admin");

        // Soft delete instead of remove
        user.IsDeleted = true;
        user.DeletedAtUtc = DateTime.UtcNow;

        // Soft delete related Patient or Doctor entity as well
        if (user.Role == UserRole.Patient)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                patient.IsDeleted = true;
                patient.DeletedAtUtc = DateTime.UtcNow;
            }
        }
        else if (user.Role == UserRole.Doctor)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor != null)
            {
                doctor.IsDeleted = true;
                doctor.DeletedAtUtc = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<AdminUserDto>> GetAllDoctorsAsync()
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.Doctor)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new AdminUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role.ToString()
            })
            .ToListAsync();
    }


    public async Task<PagedResult<AdminBookingDto>> GetAllBookingsAsync(PaginationParams pagination)
    {
        var query = _context.Bookings
            .Include(b => b.Patient).ThenInclude(p => p.User)
            .Include(b => b.Doctor).ThenInclude(d => d.User)
            .Include(b => b.TimeSlot)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(b => new AdminBookingDto
            {
                Id = b.Id,
                PatientName = b.Patient.User.FirstName + " " + b.Patient.User.LastName,
                DoctorName = b.Doctor.User.FirstName + " " + b.Doctor.User.LastName,
                StartAtUtc = b.TimeSlot.StartAtUtc,
                EndAtUtc = b.TimeSlot.EndAtUtc,
                Status = b.Status.ToString(),
                Reason = b.Reason
            })
            .ToListAsync();

        return new PagedResult<AdminBookingDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize
        };
    }
    public async Task<AdminStatsDto> GetStatsAsync()
    {
        return new AdminStatsDto
        {
            TotalPatients = await _context.Users.CountAsync(u => u.Role == UserRole.Patient),
            TotalDoctors = await _context.Users.CountAsync(u => u.Role == UserRole.Doctor),
            TotalBookings = await _context.Bookings.CountAsync(),
            PendingBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending),
            ConfirmedBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Confirmed),
            CancelledBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Cancelled)
        };
    }
}
