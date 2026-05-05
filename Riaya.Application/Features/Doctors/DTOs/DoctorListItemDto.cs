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

        // 🔥 الأساسيات للـ UI
        public string Specialization { get; set; } = string.Empty; // جاي من Doctor.Specialty
        public string University { get; set; } = string.Empty;

        // 🔥 يفرق في الكارت
        public int YearsOfExperience { get; set; }

        // 🔥 حالة الحجز
        public bool IsAvailable { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
