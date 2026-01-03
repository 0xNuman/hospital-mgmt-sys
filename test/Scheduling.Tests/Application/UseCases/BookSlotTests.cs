using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Scheduling.Application.Commands;
using Scheduling.Application.Exceptions;
using Scheduling.Application.Ports;
using Scheduling.Application.UseCases;
using Scheduling.Domain.Bookings;
using Scheduling.Domain.Slots;

namespace Scheduling.Tests.Application.UseCases;

public class BookSlotTests
{
    private readonly ISlotRepository _slotRepo;
    private readonly IBookingRepository _bookingRepo;
    private readonly BookSlot _useCase;

    public BookSlotTests()
    {
        // 1. Create Mocks
        _slotRepo = Substitute.For<ISlotRepository>();
        _bookingRepo = Substitute.For<IBookingRepository>();
        
        // 2. Instantiate Use Case with Mocks
        _useCase = new BookSlot(_slotRepo, _bookingRepo);
    }

    [Fact]
    public async Task Booking_succeeds_for_available_slot()
    {
        // Arrange
        var slotId = Guid.NewGuid();
        var slot = new Slot(slotId, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(10, 15));
        
        // Mock: Slot exists
        _slotRepo.Get(slotId).Returns(slot);

        // Act
        var command = new BookSlotCommand(slotId, Guid.NewGuid());
        var result = await _useCase.Execute(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.BookingId);
        
        // Verify we tried to add a booking
        await _bookingRepo.Received(1).Add(Arg.Is<Booking>(b => b.SlotId == slotId));
    }

    [Fact]
    public async Task Booking_fails_if_slot_not_found()
    {
        // Arrange
        var slotId = Guid.NewGuid();
        _slotRepo.Get(slotId).Returns((Slot?)null);

        // Act
        var result = await _useCase.Execute(new BookSlotCommand(slotId, Guid.NewGuid()));

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Slot not found", result.Error);
    }

    [Fact]
    public async Task Booking_fails_if_slot_is_blocked_administratively()
    {
        // Arrange
        var slotId = Guid.NewGuid();
        var slot = new Slot(slotId, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(10, 15));
        
        // Setup: Slot is technically "Blocked" in the domain
        slot.Block(); 
        
        _slotRepo.Get(slotId).Returns(slot);

        // Act
        var result = await _useCase.Execute(new BookSlotCommand(slotId, Guid.NewGuid()));

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Slot is not available", result.Error);
    }

    [Fact]
    public async Task Booking_fails_if_already_booked_concurrently()
    {
        // Arrange
        var slotId = Guid.NewGuid();
        var slot = new Slot(slotId, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(10, 15));
        
        _slotRepo.Get(slotId).Returns(slot);

        // Mock: When BookingRepo.Add is called, throw the specific Domain Exception
        // This simulates the database unique constraint violation
        _bookingRepo
            .When(x => x.Add(Arg.Any<Booking>()))
            .Do(x => throw new SlotAlreadyBookedException());

        // Act
        var result = await _useCase.Execute(new BookSlotCommand(slotId, Guid.NewGuid()));

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Slot is already booked", result.Error);
    }
}