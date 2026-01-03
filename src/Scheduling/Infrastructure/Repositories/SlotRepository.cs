using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Ports;
using Scheduling.Domain.Slots;

namespace Scheduling.Infrastructure.Repositories;

public sealed class SlotRepository(SchedulingDbContext db) : ISlotRepository
{
    public async Task<Slot?> Get(Guid slotId)
        => await db.Slots.SingleOrDefaultAsync(s => s.Id == slotId);

    public async Task<IReadOnlyList<Slot>> GetAvailableSlots(Guid doctorId, DateOnly date)
    {
        return await db.Slots
            .AsNoTracking()
            .Where(s =>
                s.DoctorId == doctorId &&
                s.Date == date &&
                s.Status == SlotStatus.Available)
            .ToListAsync();
    }

    public async Task Add(Slot slot)
    {
        db.Slots.Add(slot);
        await db.SaveChangesAsync();
    }

    public async Task Update(Slot slot)
    {
        db.Slots.Update(slot);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Slot>> GetInRange(Guid doctorId, DateOnly from, DateOnly to)
        => await db.Slots
            .AsNoTracking()
            .Where(s =>
                s.DoctorId == doctorId &&
                s.Date >= from &&
                s.Date <= to)
            .ToListAsync();

    public async Task AddBatch(IEnumerable<Slot> slots)
    {
        db.Slots.AddRange(slots);
        await db.SaveChangesAsync();
    }
}