using Scheduling.Application.Commands;
using Scheduling.Application.UseCases;
using Web.Dtos;

namespace Web.Endpoints;

public static class BlockingEndpoints
{
    public static IEndpointRouteBuilder MapBlockingEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/doctors/{doctorId:guid}/slots/{slotId:guid}/block",
                BlockSlot)
            .WithName("BlockSlot");

        return app;
    }

    private static async Task<IResult> BlockSlot(
        Guid doctorId,
        Guid slotId,
        AdminBlockSlot useCase)
    {
        var command = new BlockSlotCommand(slotId);
        await useCase.Execute(command);

        // Block ALWAYS wins
        return Results.Ok(ApiResult.Ok());
    }
}