using Scheduling.Application.UseCases;

namespace Web.Workers;

public class SlotGenerationWorker(IServiceProvider serviceProvider, ILogger<SlotGenerationWorker> logger)
    : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromHours(12);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Slight delay to allow app to start up fully
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("✨ Auto-Magic Slot Generation Started...");

            try
            {
                using var scope = serviceProvider.CreateScope();
                var useCase = scope.ServiceProvider.GetRequiredService<GenerateFutureSlots>();

                await useCase.Execute();

                logger.LogInformation("✅ Slot Generation Completed Successfully.");
            }
            catch (Exception e)
            {
                logger.LogError(e, "❌ Slot Generation Failed.");
            }

            // Wait until next interval
            using var timer = new PeriodicTimer(_interval);
            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }
}