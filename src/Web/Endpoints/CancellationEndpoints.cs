using Scheduling.Application.Commands;
using Scheduling.Application.UseCases;
using Web.Dtos;

namespace Web.Endpoints;

public static class CancellationEndpoints
{
    public static IEndpointRouteBuilder MapCancellationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/doctors/{doctorId:guid}/slots/{slotId:guid}/cancel", CancelBooking).WithName("CancelBooking");
        return app;
    }

    private static async Task<IResult> CancelBooking(
        Guid doctorId,
        Guid slotId,
        CancelBooking useCase
    )
    {
        var command = new CancelBookingCommand(slotId);
        var result = await useCase.Execute(command);

        return result.IsSuccess ? Results.Ok() : Results.BadRequest(ApiResult.Fail("CancelFailed", result.Error!));
    }
}