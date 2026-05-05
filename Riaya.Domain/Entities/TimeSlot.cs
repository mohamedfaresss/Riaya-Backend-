using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Riaya.Domain.Entities
{
    public class TimeSlot : BaseEntity
    {
        public Guid ClinicId { get; set; }
        public Guid DoctorId { get; set; }

        public DateTime StartAtUtc { get; set; }
        public DateTime EndAtUtc { get; set; }

        public Clinic Clinic { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;

        public Booking? Booking { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAtUtc { get; set; }
        public decimal Price { get; set; } = 100; // default سعر الكشف

    }
}
