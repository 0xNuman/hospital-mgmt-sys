using ReferenceData.Application.UseCases;
using Web.Dtos.Admin;

namespace Web.Endpoints.Admin;

public static class AdminDoctorEndpoints
{
    public static IEndpointRouteBuilder MapAdminDoctorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/admin/doctors", CreateDoctor);
        app.MapGet("/api/admin/doctors", GetDoctors);
        return app;
    }

    private static async Task<IResult> CreateDoctor(
        CreateDoctorRequest request,
        CreateDoctor useCase)
    {
        var doctor = await useCase.Execute(
            request.FullName,
            request.Specialty);

        return Results.Ok(doctor);
    }

    private static async Task<IResult> GetDoctors(
        ReferenceData.Application.Ports.IDoctorRepository repo)
    {
        return Results.Ok(await repo.GetAllActive());
    }
}