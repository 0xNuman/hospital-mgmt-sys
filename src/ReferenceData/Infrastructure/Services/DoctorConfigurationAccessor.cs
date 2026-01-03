using Microsoft.EntityFrameworkCore;
using Scheduling.Application.Ports;

namespace ReferenceData.Infrastructure.Services;

public sealed class DoctorConfigurationAccessor(ReferenceDataDbContext db) : IDoctorConfigurationAccessor
{
    public async Task<IReadOnlyList<DoctorScheduleConfig>> GetAllActiveConfigs()
    {
        var query = from d in db.Doctors
            join a in db.DoctorAvailabilities on d.Id equals a.DoctorId
            where d.IsActive && a.IsActive
            select new { d.Id, a };

        var results = await query.ToListAsync();

        return results.Select(r => new DoctorScheduleConfig(
            r.Id,
            r.a.WorkingDays.ToHashSet(),
            r.a.DailyStartTime,
            r.a.DailyEndTime,
            r.a.RollingWindowDays,
            r.a.SlotDurationMinutes
        )).ToList();
    }
}