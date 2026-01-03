using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Availability;
using Scheduling.Domain.Bookings;
using Scheduling.Domain.Slots;

namespace Scheduling.Infrastructure;

public sealed class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    public DbSet<Slot> Slots => Set<Slot>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<AvailabilityException> AvailabilityExceptions => Set<AvailabilityException>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x =>
                    new { x.DoctorId, x.Date, x.StartTime })
                .IsUnique();

            entity.Property(x => x.StartTime)
                .HasConversion(
                    t => t.ToTimeSpan(),
                    t => TimeOnly.FromTimeSpan(t));

            entity.Property(x => x.EndTime)
                .HasConversion(
                    t => t.ToTimeSpan(),
                    t => TimeOnly.FromTimeSpan(t));
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(b => b.SlotId).IsRequired();

            entity.Property(b => b.PatientId).IsRequired();

            entity.Property(b => b.Status).HasConversion<string>().IsRequired();

            entity.HasIndex(b => b.SlotId).IsUnique().HasFilter("Status = 'Active'");
        });

        modelBuilder.Entity<AvailabilityException>(entity =>
        {
            entity.HasKey(x => new { x.DoctorId, x.Date });

            entity.Property(x => x.Type)
                .HasConversion<string>();

            entity.Property(x => x.StartTime)
                .HasConversion<TimeSpan?>(
                    t => t.HasValue ? t.Value.ToTimeSpan() : null,
                    t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);

            entity.Property(x => x.EndTime)
                .HasConversion<TimeSpan?>(
                    t => t.HasValue ? t.Value.ToTimeSpan() : null,
                    t => t.HasValue ? TimeOnly.FromTimeSpan(t.Value) : null);
        });
    }
}