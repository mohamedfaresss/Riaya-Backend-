using Riaya.Domain.Entities;
using Riaya.Domain.Enums;
using System.Numerics;

public class User : BaseEntity
{
    public Guid ClinicId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public Clinic Clinic { get; set; } = null!;

    // Relations
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAtUtc { get; set; }

}