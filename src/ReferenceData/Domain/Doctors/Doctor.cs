namespace ReferenceData.Domain.Doctors;

public sealed class Doctor
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Specialty { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}