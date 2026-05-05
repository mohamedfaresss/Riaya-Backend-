using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        public Guid ClinicId { get; set; }
        public Guid UserId { get; set; }

        public string Specialty { get; set; } = string.Empty;

        public string University { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }

        public Clinic Clinic { get; set; } = null!;
        public User User { get; set; } = null!;

        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAtUtc { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
