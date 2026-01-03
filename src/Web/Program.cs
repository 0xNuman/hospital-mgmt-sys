using ReferenceData.Infrastructure;
using Scalar.AspNetCore;
using Scheduling.Infrastructure;
using Web.Middlewares;
using Web.Modules;
using Web.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddReferenceDataApplication()
    .AddReferenceDataInfrastructure(builder.Configuration)
    .AddSchedulingApplication()
    .AddSchedulingInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHostedService<SlotGenerationWorker>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ReferenceDataSeeder>().SeedAsync();
    await scope.ServiceProvider.GetRequiredService<SchedulingDbContext>().Database.EnsureCreatedAsync();

    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("HMS API Reference")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

//app.UseHttpsRedirection();

app.MapSchedulingModule();
app.MapReferenceDataModule();

app.Run();