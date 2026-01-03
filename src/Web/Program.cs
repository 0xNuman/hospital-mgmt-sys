using ReferenceData.Infrastructure;
using Scheduling.Infrastructure;
using Web.Modules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddReferenceDataApplication()
    .AddReferenceDataInfrastructure(builder.Configuration)
    .AddSchedulingApplication()
    .AddSchedulingInfrastructure(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ReferenceDataSeeder>().SeedAsync();
    await scope.ServiceProvider.GetRequiredService<SchedulingDbContext>().Database.EnsureCreatedAsync();

    app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

//app.UseHttpsRedirection();

app.MapSchedulingModule();
app.MapReferenceDataModule();

app.Run();