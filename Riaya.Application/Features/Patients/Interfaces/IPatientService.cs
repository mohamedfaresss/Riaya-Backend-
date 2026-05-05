using Riaya.Application.Features.Patients.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Patients.Interfaces
{
    public interface IPatientService
    {
        Task<PatientProfileDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdatePatientProfileRequest request);
    }

}
