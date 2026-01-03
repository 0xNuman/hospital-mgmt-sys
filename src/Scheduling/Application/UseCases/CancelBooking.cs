using Scheduling.Application.Commands;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;

namespace Scheduling.Application.UseCases;

public sealed class CancelBooking(IBookingRepository bookings)
{
    public async Task<Result> Execute(CancelBookingCommand command)
    {
        var booking = await bookings.GetById(command.BookingId);

        if (booking is null)
        {
            return Result.Failure("Booking not found");
        }

        try
        {
            booking.Cancel();
        }
        catch (InvalidOperationException e)
        {
            return Result.Failure(e.Message);
        }

        await bookings.Update(booking);

        return Result.Success();
    }
}