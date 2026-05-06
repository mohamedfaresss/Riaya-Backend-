using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Doctors.DTOs
{
    public class DoctorListItemDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty; 
        public string University { get; set; } = string.Empty;

        public int YearsOfExperience { get; set; }

        public bool IsAvailable { get; set; }

        public string? ProfileImageUrl { get; set; }
        public decimal ConsultationFee { get; set; }

    }
}
