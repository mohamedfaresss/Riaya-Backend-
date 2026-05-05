using Riaya.Domain.Enums;

namespace Riaya.Application.Features.Bookings.DTOs;

public class BookingResponseDto
{
    public Guid Id { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public DateTime StartAtUtc { get; set; }
    public DateTime EndAtUtc { get; set; }
    public string? Reason { get; set; }
}
