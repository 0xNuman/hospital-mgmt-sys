using Scheduling.Application.Commands;
using Scheduling.Application.Ports;
using Scheduling.Application.Results;

namespace Scheduling.Application.UseCases;

public sealed class AdminBlockSlot(ISlotRepository slots)
{
    private readonly ISlotRepository _slots = slots;

    public async Task<BookingResult> Execute(BlockSlotCommand command)
    {
        await _slots.BlockSlot(command.SlotId);
        return BookingResult.Ok();
    }
}