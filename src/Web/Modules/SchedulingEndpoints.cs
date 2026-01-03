using Web.Endpoints;

namespace Web.Modules;

public static class SchedulingEndpoints
{
    public static IEndpointRouteBuilder MapSchedulingModule(
        this IEndpointRouteBuilder app)
    {
        app.MapBookingEndpoints();
        app.MapCancellationEndpoints();
        app.MapBlockingEndpoints();

        return app;
    }
}