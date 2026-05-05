using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Schedule.DTOs
{
    public class ScheduleDto
    {
        public Guid Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}
