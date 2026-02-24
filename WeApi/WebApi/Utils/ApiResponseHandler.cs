using Microsoft.AspNetCore.Mvc;
using Shared.ResultPattern;
using System.Net;

namespace WebApi.Utils;

internal static class ApiResponseHandler
{
    public static ActionResult<Result<T>> GetResponse<T>(this Result<T> result)
    {
        if (result.IsFailure)
        {
            int errorCode = result?.Error?.Code ?? (int)HttpStatusCode.InternalServerError;
            return new ObjectResult(result) { StatusCode = errorCode };
        }
        return new OkObjectResult(result);
    }

    public static ActionResult<Result<List<T>>> GetResponse<T>(this Result<List<T>> result)
    {
        if (result.IsFailure)
        {
            int errorCode = result?.Error?.Code ?? (int)HttpStatusCode.InternalServerError;
            return new ObjectResult(result) { StatusCode = errorCode };
        }
        return new OkObjectResult(result);
    }
}
