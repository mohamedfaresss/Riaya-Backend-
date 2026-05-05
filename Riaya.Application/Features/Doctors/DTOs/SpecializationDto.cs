using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Application.Features.Doctors.DTOs
{
    public class SpecializationDto
    {
        public string Value { get; set; } = string.Empty; // key
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
    }
}
