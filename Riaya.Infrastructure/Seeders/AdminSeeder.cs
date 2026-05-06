using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using Riaya.Domain.Entities;
using Riaya.Domain.Enums;
using Riaya.Persistence.Context;

namespace Riaya.Infrastructure.Seeders;

public static class AdminSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var clinic = await context.Clinics.FirstOrDefaultAsync();

        if (clinic == null)
        {
            clinic = new Clinic
            {
                Name = "Main Clinic"
            };

            context.Clinics.Add(clinic);
            await context.SaveChangesAsync();
        }

        // ── Seed Admin ────────────────────────────────────────

        var adminExists = await context.Users
            .AnyAsync(u => u.Role == UserRole.Admin);

        if (!adminExists)
        {
            var admin = new User
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@riaya.com",
                PasswordHash = BC.HashPassword("Admin@1234"),
                Role = UserRole.Admin,
                ClinicId = clinic.Id
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }

        // ── Seed Doctors ──────────────────────────────────────

        var doctorsExist = await context.Users
            .AnyAsync(u => u.Role == UserRole.Doctor);

        if (doctorsExist)
            return;

        var doctors = new[]
        {
            new
            {
                FirstName = "Ahmed",
                LastName = "El-Sayed",
                Email = "ahmed.elsayed@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Cardiology",
                University = "Cairo University",
                YearsOfExperience = 14,
                ConsultationFee = 500m
            },

            new
            {
                FirstName = "Mona",
                LastName = "Hassan",
                Email = "mona.hassan@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Dermatology",
                University = "Ain Shams University",
                YearsOfExperience = 9,
                ConsultationFee = 400m
            },

            new
            {
                FirstName = "Khaled",
                LastName = "Ibrahim",
                Email = "khaled.ibrahim@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Neurology",
                University = "Alexandria University",
                YearsOfExperience = 17,
                ConsultationFee = 600m
            },

            new
            {
                FirstName = "Sara",
                LastName = "Mahmoud",
                Email = "sara.mahmoud@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Pediatrics",
                University = "Mansoura University",
                YearsOfExperience = 11,
                ConsultationFee = 350m
            },

            new
            {
                FirstName = "Omar",
                LastName = "Farouk",
                Email = "omar.farouk@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Orthopedics",
                University = "Cairo University",
                YearsOfExperience = 13,
                ConsultationFee = 450m
            },

            new
            {
                FirstName = "Nadia",
                LastName = "Khalil",
                Email = "nadia.khalil@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Ophthalmology",
                University = "Ain Shams University",
                YearsOfExperience = 8,
                ConsultationFee = 380m
            },

            new
            {
                FirstName = "Youssef",
                LastName = "Mostafa",
                Email = "youssef.mostafa@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Dentistry",
                University = "Tanta University",
                YearsOfExperience = 7,
                ConsultationFee = 300m
            },

            new
            {
                FirstName = "Heba",
                LastName = "Samir",
                Email = "heba.samir@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Psychiatry",
                University = "Mansoura University",
                YearsOfExperience = 12,
                ConsultationFee = 550m
            },

            new
            {
                FirstName = "Tarek",
                LastName = "Nour",
                Email = "tarek.nour@riaya.com",
                Password = "Doctor@1234",
                Specialty = "ENT",
                University = "Alexandria University",
                YearsOfExperience = 10,
                ConsultationFee = 320m
            },

            new
            {
                FirstName = "Dina",
                LastName = "Adel",
                Email = "dina.adel@riaya.com",
                Password = "Doctor@1234",
                Specialty = "Gynecology",
                University = "Cairo University",
                YearsOfExperience = 15,
                ConsultationFee = 480m
            }
        };

        foreach (var d in doctors)
        {
            var user = new User
            {
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                PasswordHash = BC.HashPassword(d.Password),
                Role = UserRole.Doctor,
                ClinicId = clinic.Id
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var doctor = new Doctor
            {
                UserId = user.Id,
                ClinicId = clinic.Id,
                Specialty = d.Specialty,
                University = d.University,
                YearsOfExperience = d.YearsOfExperience,
                ConsultationFee = d.ConsultationFee
            };

            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            // ── Seed TimeSlots ───────────────────────────────

            var today = DateTime.UtcNow.Date;

            var hours = new[]
            {
                9, 10, 11,
                14, 15, 16
            };

            for (int day = 1; day <= 7; day++)
            {
                var slotDate = today.AddDays(day);

                foreach (var hour in hours)
                {
                    context.TimeSlots.Add(new TimeSlot
                    {
                        DoctorId = doctor.Id,
                        ClinicId = clinic.Id,
                        StartAtUtc = slotDate.AddHours(hour),
                        EndAtUtc = slotDate.AddHours(hour + 1),
                        Price = 100,
                        IsDeleted = false,
                        CreatedAtUtc = DateTime.UtcNow
                    });
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
