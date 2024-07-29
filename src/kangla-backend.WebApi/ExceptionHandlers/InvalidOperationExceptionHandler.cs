using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public class InvalidOperationExceptionHandler : IExceptionHandler
{
    private readonly ILogger<InvalidOperationExceptionHandler> _logger;

    public InvalidOperationExceptionHandler(ILogger<InvalidOperationExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An invalid operation was attempted");

        if (exception is InvalidOperationException)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = exception.GetType().Name,
                Title = "Invalid Operation",
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
        return false;
    }
}
