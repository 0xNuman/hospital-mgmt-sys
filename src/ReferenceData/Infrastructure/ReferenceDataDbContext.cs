using Microsoft.EntityFrameworkCore;
using ReferenceData.Domain.Doctors;
using ReferenceData.Domain.Patients;

namespace ReferenceData.Infrastructure;

public sealed class ReferenceDataDbContext(DbContextOptions<ReferenceDataDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Patient>()
            .HasKey(p => p.Id);
    }
}