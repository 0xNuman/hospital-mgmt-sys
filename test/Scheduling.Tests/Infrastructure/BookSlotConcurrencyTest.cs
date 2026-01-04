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

public sealed class BookSlotConcurrencyTests
{
    [Fact]
    public async Task Only_one_active_booking_can_be_created_for_a_slot()
    {
        // Arrange
        var connectionString = $"DataSource=file:{Guid.NewGuid()}?mode=memory&cache=shared";
        await using var keepAliveConnection = new SqliteConnection(connectionString);
        await keepAliveConnection.OpenAsync();

        var options = new DbContextOptionsBuilder<SchedulingDbContext>()
            .UseSqlite(connectionString)
            .Options;

        await using (var setupContext = new SchedulingDbContext(options))
        {
            await setupContext.Database.EnsureCreatedAsync();

            var slot = new Slot(
                Guid.NewGuid(),
                doctorId: Guid.NewGuid(),
                date: DateOnly.FromDateTime(DateTime.UtcNow),
                startTime: new TimeOnly(10, 0),
                endTime: new TimeOnly(10, 15));

            setupContext.Slots.Add(slot);
            await setupContext.SaveChangesAsync();
        }

        async Task<BookSlotResult> ExecuteBooking()
        {
            await using var context = new SchedulingDbContext(options);

            var slotRepo = new SlotRepository(context);
            var bookingRepo = new BookingRepository(context);
            var unitOfWork = new SchedulingUnitOfWork(context);

            var useCase = new BookSlot(slotRepo, bookingRepo, unitOfWork);

            return await useCase.Execute(
                new BookSlotCommand(
                    context.Slots.Single().Id,
                    Guid.NewGuid()));
        }

        // Act (two concurrent executions)
        var results = await Task.WhenAll(
            ExecuteBooking(),
            ExecuteBooking());

        // Assert
        Assert.Equal(2, results.Length);
        Assert.Single(results, r => r.Success);
        Assert.Single(results, r => !r.Success);

        await using var assertContext = new SchedulingDbContext(options);
        var activeBookings = await assertContext.Bookings
            .Where(b => b.Status == BookingStatus.Active)
            .ToListAsync();

        Assert.NotNull(activeBookings);
        Assert.NotEmpty(activeBookings);
        Assert.Single(activeBookings);
    }
}