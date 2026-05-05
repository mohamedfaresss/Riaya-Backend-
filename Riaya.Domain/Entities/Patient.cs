using System;
using System.Collections.Generic;
using System.Text;

namespace Riaya.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public Guid ClinicId { get; set; }
        public Guid UserId { get; set; }

        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        public Clinic Clinic { get; set; } = null!;
        public User User { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAtUtc { get; set; }
    }
}
