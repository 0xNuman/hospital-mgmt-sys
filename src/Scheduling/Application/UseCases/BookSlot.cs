using Scheduling.Application.Commands;
using Scheduling.Application.Exceptions;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;
using Scheduling.Domain.Bookings;
using Scheduling.Domain.Slots;

namespace Scheduling.Application.UseCases;

public sealed class BookSlot(ISlotRepository slots, IBookingRepository bookings)
{
    public async Task<BookSlotResult> Execute(
        BookSlotCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var slot = await slots.Get(command.SlotId);

        if (slot is null)
        {
            return BookSlotResult.SlotNotFound();
        }

        if (slot.Status != SlotStatus.Available)
        {
            return BookSlotResult.SlotUnavailable("Slot is not available");
        }

        var booking = new Booking(Guid.NewGuid(), command.SlotId, command.PatientId);

        try
        {
            await bookings.Add(booking);
            return BookSlotResult.Ok(booking.Id);
        }
        catch (SlotAlreadyBookedException)
        {
            return BookSlotResult.SlotUnavailable("Slot is already booked");
        }
    }
}