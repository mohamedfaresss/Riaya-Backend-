using Riaya.Domain.Enums;

namespace Riaya.Application.Features.Bookings.DTOs;

public class UpdateBookingStatusDto
{
    public BookingStatus NewStatus { get; set; }
}
