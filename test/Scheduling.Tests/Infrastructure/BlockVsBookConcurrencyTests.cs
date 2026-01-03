using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Commands;
using Scheduling.Application.UseCases;
using Scheduling.Domain;
using Scheduling.Infrastructure;

namespace Scheduling.Tests.Infrastructure;

public class BlockVsBookConcurrencyTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<SchedulingDbContext> _options;

    public BlockVsBookConcurrencyTests()
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
    public async Task Block_always_wins_over_booking()
    {
        var slotId = Guid.NewGuid();
        SeedAvailableSlot(slotId);

        var bookTask = Task.Run(() => TryBook(slotId));
        var blockTask = Task.Run(() => TryBlock(slotId));

        await Task.WhenAll(bookTask, blockTask);

        await using var verify = new SchedulingDbContext(_options);
        var slot = verify.Slots.Single(s => s.Id == slotId);

        Assert.Equal((int)SlotStatus.Blocked, slot.Status);
        Assert.Null(slot.AppointmentId);
    }

    // ---------- helpers ----------

    private async Task TryBook(Guid slotId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        var repo = new SlotRepository(ctx);
        var uc = new BookSlot(repo);

        await uc.Execute(new BookSlotCommand(slotId, Guid.NewGuid()));
    }

    private async Task TryBlock(Guid slotId)
    {
        await using var ctx = new SchedulingDbContext(_options);
        var repo = new SlotRepository(ctx);
        var uc = new AdminBlockSlot(repo);

        await uc.Execute(new BlockSlotCommand(slotId));
    }

    private void SeedAvailableSlot(Guid slotId)
    {
        using var ctx = new SchedulingDbContext(_options);
        ctx.Slots.Add(new SlotEntity
        {
            Id = slotId,
            DoctorId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(15),
            Status = (int)SlotStatus.Available,
            AppointmentId = null
        });
        ctx.SaveChanges();
    }

    public void Dispose() => _connection.Dispose();
}