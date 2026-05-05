using Riaya.Application.Features.TimeSlots.DTOs;

namespace Riaya.Application.Features.TimeSlots.Interfaces;

public interface ITimeSlotService
{
    Task<Guid> CreateTimeSlotAsync(Guid userId, Guid clinicId, CreateTimeSlotRequest request);
    Task<List<TimeSlotDto>> GetAvailableSlotsAsync(Guid doctorId);
}
