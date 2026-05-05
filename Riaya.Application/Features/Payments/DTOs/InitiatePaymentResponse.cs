using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Payments.DTOs
{
    public class InitiatePaymentResponse
    {
        public string PaymentKey { get; set; } = string.Empty;
        public string IframeUrl { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
    }
}
