using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Payments.DTOs
{

    public class InitiatePaymentRequest
    {
        public Guid TimeSlotId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
