using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Commands;
using Scheduling.Application.Results;
using Scheduling.Application.UseCases;
using Scheduling.Domain.Bookings;
using Scheduling.Domain.Slots;
using Scheduling.Infrastructure;
using Scheduling.Infrastructure.Repositories;

namespace Scheduling.Tests.Infrastructure;

public class CancelVsBookConcurrencyTests : IDisposable
{
    private readonly SqliteConnection _keepAliveConnection;
    private readonly DbContextOptions<SchedulingDbContext> _options;

    public CancelVsBookConcurrencyTests()
    {
        // specific name to avoid collision with other tests if running in parallel
        var connectionString = $"DataSource=file:{Guid.NewGuid()}?mode=memory&cache=shared";
        
        _keepAliveConnection = new SqliteConnection(connectionString);
        _keepAliveConnection.Open();

        _options = new DbContextOptionsBuilder<SchedulingDbContext>()
            .UseSqlite(connectionString)
            .Options;

        using var ctx = new SchedulingDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    [Fact]
    public async Task Cancel_and_book_race_ends_in_consistent_state()
    {
        // Arrange
        var slotId = Guid.NewGuid();
        var initialBookingId = Guid.NewGuid();
        await SeedBookedSlot(slotId, initialBookingId);

        // Act: Race Condition
        // T1: Try to book the slot (Patient B)
        var bookTask = Task.Run(() => TryBook(slotId));
        // T2: Try to cancel the existing booking (Patient A)
        var cancelTask = Task.Run(() => TryCancel(initialBookingId));

        await Task.WhenAll(bookTask, cancelTask);

        // Assert
        await using var verify = new SchedulingDbContext(_options);

        // 1. Check Slot Status (Should always be Available in the new model, as logic is in Booking)
        var slot = await verify.Slots.SingleAsync(s => s.Id == slotId);
        Assert.Equal(SlotStatus.Available, slot.Status);

        // 2. Check Bookings
        var bookings = await verify.Bookings.Where(b => b.SlotId == slotId).ToListAsync();

        var activeBookings = bookings.Count(b => b.Status == BookingStatus.Active);
        Assert.True(activeBookings <= 1, "There should never be more than 1 active booking");

        // Possible Outcomes:
        // A) Cancel won race, Book won race -> Booking A Cancelled, Booking B Active (Valid)
        // B) Cancel won race, Book lost race (e.g. read stale data) -> Booking A Cancelled, No Active (Valid)
        // C) Book ran before Cancel? -> Impossible, Book would fail immediately as Slot was active.
    }

    private async Task<BookSlotResult> TryBook(Guid slotId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        var slotRepo = new SlotRepository(ctx);
        var bookingRepo = new BookingRepository(ctx);
        var unitOfWork = new SchedulingUnitOfWork(ctx);
        var uc = new BookSlot(slotRepo, bookingRepo, unitOfWork);

        return await uc.Execute(new BookSlotCommand(slotId, Guid.NewGuid()));
    }

    private async Task<Result> TryCancel(Guid bookingId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        var bookingRepo = new BookingRepository(ctx);
        var slotRepo = new SlotRepository(ctx);
        var unitOfWork = new SchedulingUnitOfWork(ctx);
        
        // We need ISlotRepository now for CancelBooking
        var uc = new CancelBooking(bookingRepo, slotRepo, unitOfWork);

        return await uc.Execute(new CancelBookingCommand(bookingId));
    }

    private async Task SeedBookedSlot(Guid slotId, Guid bookingId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        // Create the Slot
        ctx.Slots.Add(new Slot(slotId, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), new TimeOnly(10, 0),
            new TimeOnly(11, 0)));
        // Create the Booking
        ctx.Bookings.Add(new Booking(bookingId, slotId, Guid.NewGuid()));
        await ctx.SaveChangesAsync();
    }

    public void Dispose() => _keepAliveConnection.Dispose();
}