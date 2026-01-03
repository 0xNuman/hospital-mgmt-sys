using ReferenceData.Application.Ports;
using ReferenceData.Domain.Doctors;

namespace ReferenceData.Application.UseCases;

public sealed class CreateDoctor(IDoctorRepository doctors)
{
    public async Task<Doctor> Execute(string fullName, string speciality)
    {
        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Specialty = speciality,
            IsActive = true
        };
        
        await doctors.Add(doctor);
        return doctor;
    }
}