using ReferenceData.Domain.Doctors;
using ReferenceData.Domain.Patients;

namespace ReferenceData.Infrastructure;

public sealed class ReferenceDataSeeder(ReferenceDataDbContext db)
{
    public async Task SeedAsync()
    {
        await db.Database.EnsureCreatedAsync();

        if (db.Doctors.Any())
        {
            return;
        }

        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            FullName = "Dr. Anand Nagrani",
            Specialty = "Orthopaedic Surgeon",
            IsActive = true
        };

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FullName = "Ved Prakash",
            Phone = "9000000000",
            Email = "ved.prakash@udayclinics.com"
        };

        await db.Doctors.AddAsync(doctor);
        await db.Patients.AddAsync(patient);
        await db.SaveChangesAsync();
    }
}