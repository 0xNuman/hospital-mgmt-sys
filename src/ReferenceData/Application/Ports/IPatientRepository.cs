using ReferenceData.Domain.Patients;

namespace ReferenceData.Application.Ports;

public interface IPatientRepository
{
    Task<Patient?> GetById(Guid patientId);
    
    Task<IReadOnlyList<Patient>> GetAll();
    
    Task<Patient?> GetByPhone(string phone);

    Task Add(Patient patient);
}