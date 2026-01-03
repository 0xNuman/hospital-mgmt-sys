using Scheduling.Application.Ports;

namespace Scheduling.Application.UseCases;

public sealed record SlotDto(Guid SlotId, string Start, string End);

public sealed class GetAvailableSlots(ISlotRepository slots)
{
    public async Task<IReadOnlyList<SlotDto>> Execute(Guid doctorId, DateOnly? date)
    {
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var entities = await slots.GetAvailableSlots(doctorId, targetDate);

        var result = entities.OrderBy(s => s.StartTime)
            .Select(s => new SlotDto(
                s.Id,
                s.StartTime.ToString("HH:mm"),
                s.EndTime.ToString("HH:mm")))
            .ToList();

        return result;
    }
}