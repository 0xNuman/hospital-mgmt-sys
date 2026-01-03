namespace Scheduling.Application.Commands;

public sealed record BookSlotCommand(
    Guid SlotId,
    Guid PatientId
);