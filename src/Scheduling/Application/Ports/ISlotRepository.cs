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

    Task<IReadOnlyList<Slot>> GetInRange(Guid doctorId, DateOnly from, DateOnly to);

    Task AddBatch(IEnumerable<Slot> slots);
}