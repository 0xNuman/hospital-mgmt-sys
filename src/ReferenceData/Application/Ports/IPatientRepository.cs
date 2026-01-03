using ReferenceData.Domain.Patients;

namespace ReferenceData.Application.Ports;

public interface IPatientRepository
{
    Task<Patient?> GetById(Guid patientId);
    
    Task<IReadOnlyList<Patient>> GetAll();
    
    Task Add(Patient patient);
}