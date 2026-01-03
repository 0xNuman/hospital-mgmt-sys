using Microsoft.EntityFrameworkCore;
using ReferenceData.Application.Ports;
using ReferenceData.Domain.Doctors;

namespace ReferenceData.Infrastructure.Repositories;

public sealed class DoctorRepository(ReferenceDataDbContext db) : IDoctorRepository
{
    public async Task<Doctor?> GetById(Guid doctorId)
        => await db.Doctors.SingleOrDefaultAsync(d => d.Id == doctorId && d.IsActive);

    public async Task<IReadOnlyList<Doctor>> GetAllActive()
        => await db.Doctors.Where(d => d.IsActive).ToListAsync();

    public async Task Add(Doctor doctor)
    {
        db.Doctors.Add(doctor);
        await db.SaveChangesAsync();
    }
}