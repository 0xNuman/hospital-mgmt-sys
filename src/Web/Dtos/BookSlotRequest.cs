namespace Web.Dtos;


// TODO: To reduce friction during booking, patientId should be optional.
// As there can be first timers and we need to think of a solution to handle this case elegantly
public sealed record BookSlotRequest(
    Guid PatientId,
    string PatientName,
    string PhoneNumber
    );