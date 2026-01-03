using Web.Endpoints;
using Web.Endpoints.Admin;

namespace Web.Modules;

public static class SchedulingEndpoints
{
    public static IEndpointRouteBuilder MapSchedulingModule(
        this IEndpointRouteBuilder app)
    {
        app.MapBookingEndpoints();
        app.MapCancellationEndpoints();
        app.MapBlockingEndpoints();

        app.MapAdminAvailabilityExceptionEndpoints();

        app.MapPublicEndpoints();

        return app;
    }
}