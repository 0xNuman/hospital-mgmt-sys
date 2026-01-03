namespace Scheduling.Application.Ports;

public interface IDoctorConfigurationAccessor
{
    // Get config for all active doctors
    Task<IReadOnlyList<DoctorScheduleConfig>> GetAllActiveConfigs();
}

public record DoctorScheduleConfig(
    Guid DoctorId,
    HashSet<DayOfWeek> WorkingDays,
    TimeOnly DailyStartTime,
    TimeOnly DailyEndTime,
    int RollingWindowDays,
    int SlotDurationMinutes
);