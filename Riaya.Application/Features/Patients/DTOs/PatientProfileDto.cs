using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Patients.DTOs
{
    public class PatientProfileDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
