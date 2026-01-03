namespace Scheduling.Application.Commands;

public record CancelBookingCommand(
    Guid BookingId
);