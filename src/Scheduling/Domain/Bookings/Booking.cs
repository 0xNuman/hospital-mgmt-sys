namespace Scheduling.Domain.Bookings;

public sealed class Booking
{
    public Guid Id { get; private set; }

    public Guid SlotId { get; private set; }

    public Guid PatientId { get; private set; }

    public BookingStatus Status { get; private set; }

    private Booking()
    {
    }

    public Booking(Guid id, Guid slotId, Guid patientId)
    {
        if (slotId == Guid.Empty)
        {
            throw new ArgumentException("SlotId is required");
        }

        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("PatientId is required");
        }

        Id = id;
        SlotId = slotId;
        PatientId = patientId;
        Status = BookingStatus.Active;
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Active)
        {
            throw new InvalidOperationException("Only active bookings can be cancelled");
        }

        Status = BookingStatus.Cancelled;
    }

    public void Invalidate()
    {
        if (Status != BookingStatus.Active)
        {
            return;
        }

        Status = BookingStatus.Invalidated;
    }
}