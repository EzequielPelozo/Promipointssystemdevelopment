using FluentValidation.Results;
using Serilog;
using Shared.ResultPattern;
using System.Net;

namespace WebApi.Utils;

public static class FluentValidationToResultErrorMapper
{
    public static Result<T> MapToTypedResult<T>(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
            throw new ArgumentException("Validation result is valid, no errors to map.");

        var errorMessage = string.Join(", ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
        Log.Error("Validation failed: {Errors}", errorMessage);

        return Result<T>.Failure(new Error((int)HttpStatusCode.BadRequest, errorMessage));
    }
}
