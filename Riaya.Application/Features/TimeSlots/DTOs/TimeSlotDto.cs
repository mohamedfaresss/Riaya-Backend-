namespace Riaya.Application.Features.TimeSlots.DTOs;

public class TimeSlotDto
{
    public Guid Id { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
}
