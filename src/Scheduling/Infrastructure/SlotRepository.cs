using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Exceptions;
using Scheduling.Application.Ports;
using Scheduling.Domain;

namespace Scheduling.Infrastructure;

public sealed class SlotRepository(SchedulingDbContext db) : ISlotRepository
{
    public async Task<Slot?> GetById(Guid slotId)
    {
        var foundSlot = await db.Slots.SingleOrDefaultAsync(s => s.Id == slotId);
        return foundSlot is null ? null : MapToDomain(foundSlot);
    }

    public async Task Save(Slot slot)
    {
        // 🔒 CRITICAL SECTION
        var affected = await db.Database
            .ExecuteSqlRawAsync("""
                                UPDATE Slots
                                SET Status = {0}, AppointmentId = {1}
                                WHERE Id = {2} AND Status = {3}
                                """,
                (int)slot.Status,
                slot.AppointmentId!,
                slot.Id,
                (int)SlotStatus.Available);

        if (affected != 1 && slot.Status == SlotStatus.Booked)
        {
            throw new SlotAlreadyBookedException();
        }

        if (slot.Status != SlotStatus.Booked)
        {
            var entity = MapToEntity(slot);
            db.Slots.Update(entity);
            await db.SaveChangesAsync();
        }
    }

    public async Task CancelBooking(Guid slotId)
    {
        var affected = await db.Database
            .ExecuteSqlRawAsync("""
                                UPDATE Slots
                                SET Status = {0},
                                    AppointmentId = NULL
                                WHERE Id = {1}
                                  AND Status = {2}
                                """,
                (int)SlotStatus.Available,
                slotId,
                (int)SlotStatus.Booked);

        if (affected != 1)
            throw new SlotNotBookedException();
    }

    private static Slot MapToDomain(SlotEntity e)
    {
        var slot = Slot.CreateAvailable(
            e.Id,
            e.DoctorId,
            e.StartTime,
            e.EndTime);

        if ((SlotStatus)e.Status == SlotStatus.Booked)
            slot.Book(e.AppointmentId!.Value);

        if ((SlotStatus)e.Status == SlotStatus.Blocked)
            slot.Block();

        return slot;
    }

    private static SlotEntity MapToEntity(Slot slot)
        => new()
        {
            Id = slot.Id,
            DoctorId = slot.DoctorId,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Status = (int)slot.Status,
            AppointmentId = slot.AppointmentId
        };
}