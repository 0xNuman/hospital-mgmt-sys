using Scheduling.Application.Commands;
using Scheduling.Application.UseCases;
using Web.Dtos;

namespace Web.Endpoints;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/doctors/{doctorId:guid}/slots/{slotId:guid}/book}", BookSlot).WithName("BookSlot");

        return app;
    }

    private static async Task<IResult> BookSlot(
        Guid doctorId,
        Guid slotId,
        BookSlotRequest request,
        BookSlot useCase
    )
    {
        // NOTE: doctorId-slotId consistency check
        // (later we can optimize this with a query)
        
        // TODO: Shall we consider strongly typed Ids like DoctorId and SlotId?
        var command = new BookSlotCommand(
            slotId,
            Guid.NewGuid()
        );
        
        var result = await useCase.Execute(command);

        if (result.Success)
            return Results.Ok(ApiResult.Ok());

        return Results.Conflict(
            ApiResult.Fail(
                code: "SlotUnavailable",
                message: result.Error!
            ));
    }
}