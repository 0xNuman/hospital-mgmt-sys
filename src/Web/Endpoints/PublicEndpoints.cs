using Scheduling.Application.UseCases;

namespace Web.Endpoints;

public static class PublicEndpoints
{
    public static IEndpointRouteBuilder MapPublicEndpoints(this IEndpointRouteBuilder app)
    {
        // 1. Get List of Doctors (Re-using ReferenceData Repo directly for read simplicity)
        app.MapGet("/api/doctors", async (ReferenceData.Application.Ports.IDoctorRepository repo) =>
        {
            var doctors = await repo.GetAllActive();
            return Results.Ok(doctors);
        });

        // 2. Get Available Slots for a Doctor
        app.MapGet("/api/doctors/{doctorId:guid}/slots", async (
            Guid doctorId,
            string? date, // Format: yyyy-mm-dd
            GetAvailableSlots useCase) =>
        {
            DateOnly? targetDate = date != null ? DateOnly.Parse(date) : null;
            var slots = await useCase.Execute(doctorId, targetDate);
            return Results.Ok(slots);
        });

        // 3. Get Patient's Bookings
        app.MapGet("/api/patients/{patientId:guid}/bookings", async (
            Guid patientId,
            GetPatientBookings useCase) =>
        {
            var bookings = await useCase.Execute(patientId);
            return Results.Ok(bookings);
        });

        // 4. Lookup Patient by Phone
        app.MapPost("/api/patients/lookup", async (
            LookupPatientRequest request,
            ReferenceData.Application.Ports.IPatientRepository repo) =>
        {
            var patient = await repo.GetByPhone(request.Phone);
            if (patient == null)
            {
                return Results.NotFound(new { message = "Patient not found" });
            }
            return Results.Ok(patient);
        });

        return app;
    }
}

public record LookupPatientRequest(string Phone);