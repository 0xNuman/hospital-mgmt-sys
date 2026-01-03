using Scheduling.Domain.Slots;

namespace Scheduling.Application.Ports;

public interface ISlotRepository
{
    Task<Slot?> Get(Guid slotId);

    Task<IReadOnlyList<Slot>> GetAvailableSlots(
        Guid doctorId,
        DateOnly date);

    Task Add(Slot slot);

    Task Update(Slot slot);
}