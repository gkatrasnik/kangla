using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public class KeyNotFoundExceptionHandler : IExceptionHandler
{
    private readonly ILogger<KeyNotFoundExceptionHandler> _logger;
    public KeyNotFoundExceptionHandler(ILogger<KeyNotFoundExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Key was not found");

        if (exception is KeyNotFoundException)
        {

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Type = exception.GetType().Name,
                Title = "Key was not found",
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
        return false;
    }
}