using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Patients.DTOs
{
    public class UpdatePatientProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
