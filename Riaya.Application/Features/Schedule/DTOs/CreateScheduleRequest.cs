using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Schedule.DTOs
{

    public class CreateScheduleRequest
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 30;
    }
}
