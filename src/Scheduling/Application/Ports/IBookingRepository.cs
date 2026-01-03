using Scheduling.Domain.Bookings;

namespace Scheduling.Application.Ports;

public interface IBookingRepository
{
    Task Add(Booking booking);

    Task<Booking?> GetActiveBySlotId(Guid slotId);
    
    Task<Booking?> GetById(Guid bookingId);

    Task Update(Booking booking);
}