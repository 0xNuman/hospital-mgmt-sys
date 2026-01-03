using ReferenceData.Application.Ports;
using ReferenceData.Domain.Patients;

namespace ReferenceData.Application.UseCases;

public sealed class CreatePatient(IPatientRepository patients)
{
    public async Task<Patient> Execute(string fullName, string phone, string? email)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Phone = phone,
            Email = email
        };

        await patients.Add(patient);
        return patient;
    }
}