namespace Riaya.Application.Features.Doctors.DTOs;

public class DoctorProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string? University { get; set; }
    public string? Experience { get; set; }
    public string? ImageUrl { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}
