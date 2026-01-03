using Scheduling.Application.Ports;
using Scheduling.Domain.Availability;
using Scheduling.Domain.Slots;

namespace Scheduling.Application.UseCases;

public sealed class GenerateFutureSlots(
    IDoctorConfigurationAccessor configurationAccessor,
    IAvailabilityExceptionRepository exceptions,
    ISlotRepository slots
)
{
    public async Task Execute()
    {
        var configs = await configurationAccessor.GetAllActiveConfigs();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var config in configs)
        {
            await GenerateForDoctor(config, today);
        }
    }

    private async Task GenerateForDoctor(DoctorScheduleConfig config, DateOnly from)
    {
        var to = from.AddDays(config.RollingWindowDays);

        // 1. Fetch exceptions for this range (Leave, Sick days, etc.)
        var overrides = await exceptions.GetInRange(config.DoctorId, from, to);

        // 2. Fetch existing slots to ensure Idempotency (Don't generate duplicates)
        // Note: Consider adding GetInRange to ISlotRepository
        var existingSlots = await slots.GetAvailableSlots(config.DoctorId, from);
        var existingKeys = existingSlots.Select(s => new { s.Date, s.StartTime }).ToHashSet();

        var slotsToAdd = new List<Slot>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            // Skip non-working days
            if (!config.WorkingDays.Contains(date.DayOfWeek)) continue;

            // Skip Full day Blocks
            var dailyException = overrides.FirstOrDefault(e => e.Date == date);
            if (dailyException?.Type == AvailabilityExceptionType.FullDayBlock) continue;

            var currentTime = config.DailyStartTime;

            // Generate chunks
            while (currentTime.AddMinutes(config.SlotDurationMinutes) <= config.DailyEndTime)
            {
                var endTime = currentTime.AddMinutes(config.SlotDurationMinutes);

                // Skip if Partial Day Block overlaps
                bool isBlocked = dailyException?.Type == AvailabilityExceptionType.PartialDayBlock &&
                                 IsOverlapping(dailyException, currentTime, endTime);

                // Skip if already exists
                if (!isBlocked && !existingKeys.Contains(new { Date = date, StartTime = currentTime }))
                {
                    slotsToAdd.Add(new Slot(Guid.NewGuid(), config.DoctorId, date, currentTime, endTime));
                }

                currentTime = endTime;
            }
        }

        if (slotsToAdd.Count > 0)
        {
            await slots.AddBatch(slotsToAdd);
        }
    }

    private static bool IsOverlapping(AvailabilityException ex, TimeOnly slotStart,
        TimeOnly slotEnd)
    {
        if (ex.StartTime == null || ex.EndTime == null) return false;
        return slotStart < ex.EndTime && slotEnd > ex.StartTime;
    }
}