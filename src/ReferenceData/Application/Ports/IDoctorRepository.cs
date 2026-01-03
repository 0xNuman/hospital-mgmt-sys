using ReferenceData.Domain.Doctors;

namespace ReferenceData.Application.Ports;

public interface IDoctorRepository
{
    Task<Doctor?> GetById(Guid doctorId);

    Task<IReadOnlyList<Doctor>> GetAllActive();
    
    Task Add(Doctor doctor);
}