using Riaya.Application.Features.Payments.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Payments.Interfaces
{

    public interface IPaymentService
    {
        Task<InitiatePaymentResponse> InitiatePaymentAsync(Guid userId, Guid clinicId, InitiatePaymentRequest request);
        Task<bool> HandleCallbackAsync(PaymobCallbackDto callback);
    }
}
