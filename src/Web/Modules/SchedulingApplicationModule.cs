using Scheduling.Application.UseCases;

namespace Web.Modules;

public static class SchedulingApplicationModule
{
    public static IServiceCollection AddSchedulingApplication(
        this IServiceCollection services)
    {
        services.AddScoped<BookSlot>();
        services.AddScoped<CancelBooking>();
        services.AddScoped<AdminBlockSlot>();
        services.AddScoped<GenerateFutureSlots>();
        services.AddScoped<GetAvailableSlots>();
        services.AddScoped<GetPatientBookings>();
        
        services.AddScoped<CreateAvailabilityException>();
        services.AddScoped<DeleteAvailabilityException>();

        return services;
    }
}