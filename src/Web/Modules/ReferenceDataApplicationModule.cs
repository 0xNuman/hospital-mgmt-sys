using ReferenceData.Application.UseCases;

namespace Web.Modules;

public static class ReferenceDataApplicationModule
{
    public static IServiceCollection AddReferenceDataApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateDoctor>();
        services.AddScoped<CreatePatient>();
        return services;
    }
}