using Riaya.Application.Features.Schedule.DTOs;
using Riaya.Application.Features.TimeSlots.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Schedule.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleDto> CreateScheduleAsync(Guid userId, Guid clinicId, CreateScheduleRequest request);
        Task<List<ScheduleDto>> GetMySchedulesAsync(Guid userId);
        Task DeleteScheduleAsync(Guid userId, Guid scheduleId);
        Task<List<TimeSlotDto>> GenerateSlotsAsync(Guid userId, Guid clinicId, GenerateSlotsRequest request);
    }
}
