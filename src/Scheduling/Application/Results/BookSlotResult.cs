namespace Scheduling.Application.Results;

public sealed record BookSlotResult(
    bool Success,
    string? Error = null,
    Guid? BookingId = null
)
{
    public static BookSlotResult Ok(Guid bookingId) => new(true, null, bookingId);

    public static BookSlotResult SlotUnavailable(string message) => new(false, message);

    public static BookSlotResult SlotNotFound() => new(false, "Slot not found");
}