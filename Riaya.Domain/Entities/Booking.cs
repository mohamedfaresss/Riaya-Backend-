using Riaya.Domain.Entities;
using Riaya.Domain.Enums;
using System.Numerics;

public class Booking : BaseEntity
{
    public Guid ClinicId { get; set; }

    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid TimeSlotId { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string Reason { get; set; } = string.Empty;

    public Clinic Clinic { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAtUtc { get; set; }
}