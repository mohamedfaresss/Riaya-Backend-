using Microsoft.EntityFrameworkCore;
using Riaya.Application.Features.Bookings.DTOs;
using Riaya.Application.Features.Bookings.Interfaces;
using Riaya.Domain.Entities;
using Riaya.Domain.Exceptions;
using Riaya.Persistence.Context;

namespace Riaya.Infrastructure.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateBookingAsync(Guid userId, Guid clinicId, CreateBookingRequest request)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ClinicId == clinicId);

        if (patient == null)
            throw new NotFoundException("Patient not found");

        var slot = await _context.TimeSlots
            .Include(t => t.Booking)
            .FirstOrDefaultAsync(t => t.Id == request.TimeSlotId && t.ClinicId == clinicId);

        if (slot == null)
            throw new NotFoundException("TimeSlot not found");

        if (slot.Booking != null)
            throw new ConflictException("This slot is already booked");

        var booking = new Booking
        {
            PatientId = patient.Id,
            DoctorId = slot.DoctorId,
            TimeSlotId = slot.Id,
            ClinicId = clinicId,
            Reason = request.Reason
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking.Id;
    }

    public async Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(Guid callerId, string callerRole)
    {
        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.TimeSlot)
            .Include(b => b.Doctor)
                .ThenInclude(d => d.User)
            .Include(b => b.Patient)
                .ThenInclude(p => p.User);

        if (callerRole == "Patient")
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == callerId);

            if (patient == null)
                throw new NotFoundException("Patient not found");

            query = query.Where(b => b.PatientId == patient.Id);
        }
        else if (callerRole == "Doctor")
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == callerId);

            if (doctor == null)
                throw new NotFoundException("Doctor not found");

            query = query.Where(b => b.DoctorId == doctor.Id);
        }
        else if (callerRole != "Admin")
        {
            throw new ForbiddenException("You are not allowed to access these bookings.");
        }

        var bookings = await query
            .OrderByDescending(b => b.TimeSlot.StartAtUtc)
            .ToListAsync();

        return bookings.Select(MapBookingResponse);
    }

    public async Task CancelBookingAsync(Guid bookingId, Guid callerId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            throw new NotFoundException("Booking not found");

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == callerId);

        if (patient == null)
            throw new NotFoundException("Patient not found");

        if (booking.PatientId != patient.Id)
            throw new ForbiddenException("You are not allowed to cancel this booking.");

        if (booking.Status != Riaya.Domain.Enums.BookingStatus.Pending &&
            booking.Status != Riaya.Domain.Enums.BookingStatus.Confirmed)
        {
            throw new ConflictException("Booking cannot be cancelled in its current state.");
        }

        booking.Status = Riaya.Domain.Enums.BookingStatus.Cancelled;
        await _context.SaveChangesAsync();
    }

    public async Task<BookingResponseDto> UpdateBookingStatusAsync(Guid bookingId, Guid callerId, string callerRole, UpdateBookingStatusDto dto)
    {
        var booking = await _context.Bookings
            .Include(b => b.TimeSlot)
            .Include(b => b.Doctor)
                .ThenInclude(d => d.User)
            .Include(b => b.Patient)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            throw new NotFoundException("Booking not found");

        if (callerRole == "Doctor")
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == callerId);

            if (doctor == null)
                throw new NotFoundException("Doctor not found");

            if (booking.DoctorId != doctor.Id)
                throw new ForbiddenException("You are not allowed to update this booking.");
        }
        else if (callerRole != "Admin")
        {
            throw new ForbiddenException("You are not allowed to update this booking.");
        }

        booking.Status = dto.NewStatus;
        await _context.SaveChangesAsync();

        return MapBookingResponse(booking);
    }

    private static BookingResponseDto MapBookingResponse(Booking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            DoctorName = $"{booking.Doctor.User.FirstName} {booking.Doctor.User.LastName}",
            PatientName = $"{booking.Patient.User.FirstName} {booking.Patient.User.LastName}",
            Status = booking.Status,
            StartAtUtc = booking.TimeSlot.StartAtUtc,
            EndAtUtc = booking.TimeSlot.EndAtUtc,
            Reason = booking.Reason
        };
    }
}
