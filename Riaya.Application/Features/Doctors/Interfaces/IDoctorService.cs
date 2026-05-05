using Riaya.Application.Features.Doctors.DTOs;
using Riaya.Application.Features.TimeSlots.DTOs;

namespace Riaya.Application.Features.Doctors.Interfaces;

public interface IDoctorService
{
    Task<DoctorProfileDto> GetProfileAsync(Guid userId);
    Task UpdateProfileAsync(Guid userId, UpdateDoctorProfileRequest request);
    Task<List<DoctorListItemDto>> GetAllDoctorsAsync();
    Task<List<TimeSlotDto>> GetMyTimeSlotsAsync(Guid userId);
    Task DeleteTimeSlotAsync(Guid userId, Guid slotId);
    public Task<List<SpecializationDto>> GetSpecializationsAsync();

}
