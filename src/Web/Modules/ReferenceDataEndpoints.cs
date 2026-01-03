using Web.Endpoints.Admin;

namespace Web.Modules;

public static class ReferenceDataEndpoints
{
    public static IEndpointRouteBuilder MapReferenceDataModule(
        this IEndpointRouteBuilder app)
    {
        app.MapAdminDoctorEndpoints();
        app.MapAdminPatientEndpoints();
        return app;
    }
}