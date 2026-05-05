using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Schedule.DTOs
{
    public class GenerateSlotsRequest
    {
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
    }

}
