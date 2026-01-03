using Scheduling.Domain;

namespace Scheduling.Application.Ports;

public interface ISlotRepository
{
    Task<Slot?> GetById(Guid slotId);

    Task Save(Slot slot);
    
    Task CancelBooking(Guid slotId);
    
    Task BlockSlot(Guid slotId);
}