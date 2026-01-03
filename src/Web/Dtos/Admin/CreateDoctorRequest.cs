namespace Web.Dtos.Admin;

public sealed record CreateDoctorRequest(
    string FullName,
    string Specialty
);