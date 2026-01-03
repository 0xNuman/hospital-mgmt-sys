// using Scheduling.Application.Ports;
// using Scheduling.Domain.Slots;
//
// namespace Scheduling.Tests.Application;
//
// public class FakeSlotRepository(Slot? slot) : ISlotRepository
// {
//     public Task<Slot?> GetById(Guid slotId) => Task.FromResult(slot?.Id == slotId ? slot : null);
//
//     public Task Save(Slot slotToSave)
//     {
//         // No-op for tests
//         return Task.CompletedTask;
//     }
//
//     public Task CancelBooking(Guid slotId)
//     {
//         // No-op for now
//         return Task.CompletedTask;
//     }
//
//     public Task BlockSlot(Guid slotId)
//     {
//         // No-op for now
//         return Task.CompletedTask;
//     }
// }