using System.Net;

namespace Shared.ResultPattern;

public sealed record Error(int Code, string Message)
{
    private static readonly int NotFoundCode = (int)HttpStatusCode.NotFound;
    private static readonly int ValidationErrorCode = (int)HttpStatusCode.BadRequest;
    private static readonly int InternalServerErrorCode = (int)HttpStatusCode.InternalServerError;

    public static readonly Error? None = null;
    public static Error RecordNotFound(string message = "Not found") => new(NotFoundCode, message);
    public static Error ValidationError(string message = "Validation error") => new(ValidationErrorCode, message);
    public static Error InternalServerError(string message = "Internal Server Error") => new(InternalServerErrorCode, message);
    public static Error Conflict(string message = "Conflict") => new((int)HttpStatusCode.Conflict, message);
    public static Error Unauthorized(string message = "Unauthorized") => new((int)HttpStatusCode.Unauthorized, message);
}
