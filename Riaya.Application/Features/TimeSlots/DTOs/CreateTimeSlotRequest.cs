using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.TimeSlots.DTOs
{
    public class CreateTimeSlotRequest
    {
        public DateTime StartAtUtc { get; set; }
        public DateTime EndAtUtc { get; set; }
    }
}
