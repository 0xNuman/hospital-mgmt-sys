namespace ReferenceData.Domain.Patients;

public sealed class Patient
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public string? Email { get; init; }
}