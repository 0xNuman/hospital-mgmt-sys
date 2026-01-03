using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReferenceData.Application.Ports;
using ReferenceData.Infrastructure.Repositories;

namespace ReferenceData.Infrastructure;

public static class ReferenceDataInfrastructureModule
{
    public static IServiceCollection AddReferenceDataInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ReferenceDataDbContext>(options =>
        {
            options.UseSqlite(
                configuration.GetConnectionString("ReferenceDataDb"));
        });

        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();

        services.AddScoped<ReferenceDataSeeder>();

        return services;
    }
}