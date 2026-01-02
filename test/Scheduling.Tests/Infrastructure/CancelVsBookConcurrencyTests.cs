using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Commands;
using Scheduling.Application.Results;
using Scheduling.Application.UseCases;
using Scheduling.Domain;
using Scheduling.Infrastructure;

namespace Scheduling.Tests.Infrastructure;

public class CancelVsBookConcurrencyTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<SchedulingDbContext> _options;

    public CancelVsBookConcurrencyTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
        _connection.Open();

        _options = new DbContextOptionsBuilder<SchedulingDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = new SchedulingDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    [Fact]
    public async Task Cancel_and_book_race_ends_in_consistent_state()
    {
        var slotId = Guid.NewGuid();
        SeedBookedSlot(slotId);

        var bookTask = Task.Run(() => TryBook(slotId));
        var cancelTask = Task.Run(() => TryCancel(slotId));

        var results = await Task.WhenAll(bookTask, cancelTask);

        Assert.All(results, r => Assert.True(r.Success));

        await using var verify = new SchedulingDbContext(_options);
        var slot = verify.Slots.Single(s => s.Id == slotId);

        if (slot.Status == (int)SlotStatus.Available)
        {
            Assert.Null(slot.AppointmentId);
        }
        else if (slot.Status == (int)SlotStatus.Booked)
        {
            Assert.NotNull(slot.AppointmentId);
        }
        else
        {
            Assert.Fail("Slot ended in invalid state");
        }

        Assert.True(
            slot.Status == (int)SlotStatus.Booked ||
            slot.Status == (int)SlotStatus.Available);
    }

    // ---------------- helpers ----------------

    private async Task<BookingResult> TryBook(Guid slotId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        var repo = new SlotRepository(ctx);
        var uc = new BookSlot(repo);

        return await uc.Execute(new BookSlotCommand(
            slotId,
            Guid.NewGuid()));
    }

    private async Task<BookingResult> TryCancel(Guid slotId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        var repo = new SlotRepository(ctx);
        var uc = new CancelBooking(repo);

        return await uc.Execute(new CancelBookingCommand(slotId));
    }

    private void SeedBookedSlot(Guid slotId)
    {
        using var ctx = new SchedulingDbContext(_options);
        ctx.Slots.Add(new SlotEntity
        {
            Id = slotId,
            DoctorId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(15),
            Status = (int)SlotStatus.Booked,
            AppointmentId = Guid.NewGuid()
        });
        ctx.SaveChanges();
    }

    public void Dispose() => _connection.Dispose();
}