using Microsoft.EntityFrameworkCore;
using Riaya.Domain.Entities;

namespace Riaya.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Global Query Filters ──────────────────────────────
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Doctor>().HasQueryFilter(d => !d.IsDeleted);
        modelBuilder.Entity<Booking>().HasQueryFilter(b => !b.IsDeleted);
        modelBuilder.Entity<TimeSlot>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(r => !r.User.IsDeleted);
        modelBuilder.Entity<DoctorSchedule>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Payment>().HasQueryFilter(p => !p.Patient.IsDeleted);


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ── RefreshToken ──────────────────────────────────────
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasOne(r => r.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(r => r.UserId);
        });

        // ── DoctorSchedule ────────────────────────────────────
        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasOne(s => s.Doctor)
                  .WithMany()
                  .HasForeignKey(s => s.DoctorId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── Payment ───────────────────────────────────────────
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Amount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(p => p.Status)
                  .HasConversion<int>();

            // Payment → Patient (no cascade)
            entity.HasOne(p => p.Patient)
                  .WithMany()
                  .HasForeignKey(p => p.PatientId)
                  .OnDelete(DeleteBehavior.NoAction);


            // Payment → TimeSlot (no cascade)
            entity.HasOne(p => p.TimeSlot)
                  .WithMany()
                  .HasForeignKey(p => p.TimeSlotId)
                  .OnDelete(DeleteBehavior.NoAction);

            // Payment → Clinic (no cascade)
            entity.HasOne(p => p.Clinic)
                  .WithMany()
                  .HasForeignKey(p => p.ClinicId)
                  .OnDelete(DeleteBehavior.NoAction);

            // Payment → Booking (optional, one-to-one)
            entity.HasOne(p => p.Booking)
                  .WithOne()
                  .HasForeignKey<Payment>(p => p.BookingId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ── TimeSlot Price default ────────────────────────────
        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.Property(t => t.Price)
                  .HasColumnType("decimal(18,2)")
                  .HasDefaultValue(100m);
        });
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
    public DbSet<Payment> Payments => Set<Payment>();  // ← جديد
}
