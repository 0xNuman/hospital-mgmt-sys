using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Commands;
using Scheduling.Application.UseCases;
using Scheduling.Domain.Bookings;
using Scheduling.Domain.Slots;
using Scheduling.Infrastructure;
using Scheduling.Infrastructure.Repositories;

namespace Scheduling.Tests.Infrastructure;

public class BlockVsBookConcurrencyTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<SchedulingDbContext> _options;

    public BlockVsBookConcurrencyTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<SchedulingDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = new SchedulingDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    [Fact]
    public async Task Block_always_wins_over_booking()
    {
        var slotId = Guid.NewGuid();
        await SeedAvailableSlot(slotId);

        var bookTask = Task.Run(() => TryBook(slotId));
        var blockTask = Task.Run(() => TryBlock(slotId));

        await Task.WhenAll(bookTask, blockTask);

        await using var verify = new SchedulingDbContext(_options);

        // 1. Slot MUST be blocked
        var slot = await verify.Slots.SingleAsync(s => s.Id == slotId);
        Assert.Equal(SlotStatus.Blocked, slot.Status);

        // 2. Booking state check
        // Ideally, if the slot is blocked, there should be NO active booking.
        // However, with current optimistic concurrency, a booking might slip in.
        // This test will FAIL with the current implementation, highlighting the need 
        // for AdminBlockSlot to check for/cancel existing bookings.
        var booking = await verify.Bookings.SingleOrDefaultAsync(b => b.SlotId == slotId);

        // Strict Assertion: If Block wins, booking should not exist or be invalid.
        // (You might need to adjust logic in AdminBlockSlot to ensure this passes)
        if (booking != null)
        {
            // If a booking slipped in, is it Active? That would be a bug (Zombie booking).
            // We assert that if a booking exists, the system doesn't consider it valid (or we accept the race for now).
            // For "Block always wins", we imply the booking shouldn't happen.
            Assert.NotEqual(BookingStatus.Active, booking.Status);
        }
    }

    private async Task TryBook(Guid slotId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        var slotRepo = new SlotRepository(ctx);
        var bookingRepo = new BookingRepository(ctx);
        var uc = new BookSlot(slotRepo, bookingRepo);

        await uc.Execute(new BookSlotCommand(slotId, Guid.NewGuid()));
    }

    private async Task TryBlock(Guid slotId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        var slotRepo = new SlotRepository(ctx);
        var bookingRepo = new BookingRepository(ctx); // Create repo
        
        // Inject both repos
        var uc = new AdminBlockSlot(slotRepo, bookingRepo); 

        await uc.Execute(new BlockSlotCommand(slotId));
    }

    private async Task SeedAvailableSlot(Guid slotId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        ctx.Slots.Add(new Slot(slotId, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), new TimeOnly(10, 0),
            new TimeOnly(10, 15)));
        await ctx.SaveChangesAsync();
    }

    public void Dispose() => _connection.Dispose();
}