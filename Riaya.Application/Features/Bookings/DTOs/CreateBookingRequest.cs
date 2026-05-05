using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Bookings.DTOs
{
    public class CreateBookingRequest
    {
        public Guid TimeSlotId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
