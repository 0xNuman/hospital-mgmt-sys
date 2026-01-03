namespace Web.Dtos;

public sealed class ApiResult
{
    public bool Success { get; init; }

    public ApiError? Error { get; init; }

    public static ApiResult Ok() => new() { Success = true };

    public static ApiResult Fail(string code, string message)
        => new() { Success = false, Error = new ApiError(code, message) };
}