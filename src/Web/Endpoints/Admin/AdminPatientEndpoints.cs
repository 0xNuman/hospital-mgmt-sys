using ReferenceData.Application.UseCases;
using Web.Dtos.Admin;

namespace Web.Endpoints.Admin;

public static class AdminPatientEndpoints
{
    public static IEndpointRouteBuilder MapAdminPatientEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/admin/patients", CreatePatient);
        app.MapGet("/api/admin/patients", GetPatients);

        return app;
    }

    private static async Task<IResult> CreatePatient(
        CreatePatientRequest request,
        CreatePatient useCase)
    {
        var patient = await useCase.Execute(
            request.FullName,
            request.Phone,
            request.Email);

        return Results.Ok(patient);
    }

    private static async Task<IResult> GetPatients(
        ReferenceData.Application.Ports.IPatientRepository repo)
    {
        return Results.Ok(await repo.GetAll());
    }
}