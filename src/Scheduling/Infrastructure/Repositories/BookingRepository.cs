using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Exceptions;
using Scheduling.Application.Ports;
using Scheduling.Domain.Bookings;

namespace Scheduling.Infrastructure.Repositories;

public sealed class BookingRepository(SchedulingDbContext db) : IBookingRepository
{
    public async Task Add(Booking booking)
    {
        db.Bookings.Add(booking);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException e) when (IsUniqueConstraintViolation(e))
        {
            throw new SlotAlreadyBookedException();
        }
    }

    public async Task<Booking?> GetActiveBySlotId(Guid slotId) =>
        await db.Bookings
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.SlotId == slotId && b.Status == BookingStatus.Active);

    public async Task<Booking?> GetById(Guid bookingId)
        => await db.Bookings.FindAsync(bookingId);

    public async Task Update(Booking booking)
    {
        db.Bookings.Update(booking);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Booking>> GetByPatientId(Guid patientId)
        => await db.Bookings
            .AsNoTracking()
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.Status)
            .ToListAsync();

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is SqliteException sqliteEx)
        {
            // SQLite constraint violation error code
            return sqliteEx.SqliteErrorCode == 19;
        }

        // SQL Server (future-proofing)
        // if (ex.InnerException is SqlException sqlEx)
        // {
        //     // 2627 = Unique constraint
        //     // 2601 = Duplicate key
        //     return sqlEx.Number is 2627 or 2601;
        // }

        return false;
    }
}