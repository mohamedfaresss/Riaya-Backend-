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
        // لو Admin موجود مسبقاً مش هيضيف تاني
        var adminExists = await context.Users
            .AnyAsync(u => u.Role == UserRole.Admin);

        if (adminExists) return;

        // ضيف Clinic الأول لو مش موجود
        var clinic = await context.Clinics.FirstOrDefaultAsync();
        if (clinic == null)
        {
            clinic = new Clinic { Name = "Main Clinic" };
            context.Clinics.Add(clinic);
            await context.SaveChangesAsync();
        }

        // ضيف Admin
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
}