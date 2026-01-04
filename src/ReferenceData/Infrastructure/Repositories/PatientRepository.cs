using Microsoft.EntityFrameworkCore;
using ReferenceData.Application.Ports;
using ReferenceData.Domain.Patients;

namespace ReferenceData.Infrastructure.Repositories;

public sealed class PatientRepository(ReferenceDataDbContext db) : IPatientRepository
{
    public async Task<Patient?> GetById(Guid patientId)
        => await db.Patients.SingleOrDefaultAsync(p => p.Id == patientId);

    public async Task<IReadOnlyList<Patient>> GetAll()
        => await db.Patients.ToListAsync();

    public async Task<Patient?> GetByPhone(string phone)
        => await db.Patients.FirstOrDefaultAsync(p => p.Phone == phone);

    public async Task Add(Patient patient)
    {
        db.Patients.Add(patient);
        await db.SaveChangesAsync();
    }
}