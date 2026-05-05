using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Doctors.DTOs
{
    public class UpdateDoctorProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public int Experience
        {
            get => YearsOfExperience;
            set => YearsOfExperience = value;
        }

        public IFormFile? Image { get; set; }

    }
}
