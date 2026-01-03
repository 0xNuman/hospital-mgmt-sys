using Microsoft.EntityFrameworkCore;
using Scheduling.Application.UseCases;
using Scheduling.Infrastructure;
using Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SchedulingDbContext>(options =>
{
    options.UseSqlite("Data Source=scheduling.db");
});

builder.Services.AddScoped<SlotRepository>();
builder.Services.AddScoped<BookSlot>();
builder.Services.AddScoped<CancelBooking>();
builder.Services.AddScoped<AdminBlockSlot>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

//app.UseHttpsRedirection();

app.MapBookingEndpoints();
app.MapCancellationEndpoints();
app.MapBlockingEndpoints();

app.Run();