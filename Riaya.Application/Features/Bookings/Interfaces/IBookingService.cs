using Riaya.Application.Features.Bookings.DTOs;

namespace Riaya.Application.Features.Bookings.Interfaces;

public interface IBookingService
{
    Task<Guid> CreateBookingAsync(Guid userId, Guid clinicId, CreateBookingRequest request);
    Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(Guid callerId, string callerRole);
    Task CancelBookingAsync(Guid bookingId, Guid callerId);
    Task<BookingResponseDto> UpdateBookingStatusAsync(Guid bookingId, Guid callerId, string callerRole, UpdateBookingStatusDto dto);
}
