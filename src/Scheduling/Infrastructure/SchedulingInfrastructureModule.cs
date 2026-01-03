using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduling.Application.Ports;
using Scheduling.Application.UseCases;
using Scheduling.Infrastructure.Repositories;

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
        services.AddScoped<IAvailabilityExceptionRepository, AvailabilityExceptionRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddScoped<GenerateFutureSlots>();

        return services;
    }
}