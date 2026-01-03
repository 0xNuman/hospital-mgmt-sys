namespace Web.Dtos.Admin;

public sealed record CreatePatientRequest(
    string FullName,
    string Phone,
    string? Email
);