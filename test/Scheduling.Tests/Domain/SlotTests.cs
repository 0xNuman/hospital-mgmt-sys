// using Scheduling.Domain.Slots;
//
// namespace Scheduling.Tests.Domain;
//
// public class SlotTests
// {
//     private static Slot CreateAvailableSlot() =>
//         Slot.CreateAvailable(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now.AddMinutes(15));
//
//     [Fact]
//     public void New_slot_is_available()
//     {
//         var slot = CreateAvailableSlot();
//         Assert.Equal(SlotStatus.Available, slot.Status);
//         Assert.Null(slot.AppointmentId);
//     }
//
//     [Fact]
//     public void Can_book_available_slot()
//     {
//         var slot = CreateAvailableSlot();
//         var appointmentId = Guid.NewGuid();
//
//         slot.Book(appointmentId);
//
//         Assert.Equal(SlotStatus.Booked, slot.Status);
//         Assert.Equal(appointmentId, slot.AppointmentId);
//     }
//
//     [Fact]
//     public void Cannot_book_slot_twice()
//     {
//         var slot = CreateAvailableSlot();
//
//         slot.Book(Guid.NewGuid());
//
//         Assert.Throws<InvalidOperationException>(() => slot.Book(Guid.NewGuid()));
//     }
//
//     [Fact]
//     public void Blocked_slot_cannot_be_booked()
//     {
//         var slot = CreateAvailableSlot();
//         slot.Block();
//
//         Assert.Throws<InvalidOperationException>(() => slot.Book(Guid.NewGuid()));
//     }
//
//     [Fact]
//     public void Can_cancel_booking()
//     {
//         var slot = CreateAvailableSlot();
//         slot.Book(Guid.NewGuid());
//
//         slot.CancelBooking();
//
//         Assert.Equal(SlotStatus.Available, slot.Status);
//         Assert.Null(slot.AppointmentId);
//     }
//
//     [Fact]
//     public void Cannot_cancel_if_not_booked()
//     {
//         var slot = CreateAvailableSlot();
//
//         Assert.Throws<InvalidOperationException>(() =>
//             slot.CancelBooking());
//     }
//
//     [Fact]
//     public void Cannot_block_booked_slot()
//     {
//         var slot = CreateAvailableSlot();
//         slot.Book(Guid.NewGuid());
//
//         Assert.Throws<InvalidOperationException>(() =>
//             slot.Block());
//     }
//
//     [Fact]
//     public void Invalid_time_range_throws()
//     {
//         Assert.Throws<ArgumentException>(() =>
//             Slot.CreateAvailable(
//                 Guid.NewGuid(),
//                 Guid.NewGuid(),
//                 DateTime.UtcNow,
//                 DateTime.UtcNow));
//     }
// }