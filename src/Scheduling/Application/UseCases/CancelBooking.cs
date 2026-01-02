using Scheduling.Application.Commands;
using Scheduling.Application.Exceptions;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;

namespace Scheduling.Application.UseCases;

public sealed class CancelBooking(ISlotRepository slots)
{
    public async Task<BookingResult> Execute(CancelBookingCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        try
        {
            await slots.CancelBooking(command.SlotId);
            return BookingResult.Ok();
        }
        catch (SlotNotBookedException e)
        {
            return BookingResult.Fail(e.Message);
        }
    }
}