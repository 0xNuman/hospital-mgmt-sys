using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduling.Application.Ports;

namespace Scheduling.Infrastructure;

public static class SchedulingInfrastructureModule
{
    public static IServiceCollection AddSchedulingInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SchedulingDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("SchedulingDb"));
        });

        services.AddScoped<ISlotRepository, SlotRepository>();

        return services;
    }
}