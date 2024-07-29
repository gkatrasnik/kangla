using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public class TimeOutExceptionHandler : IExceptionHandler
{
    private readonly ILogger<TimeOutExceptionHandler> _logger;
    public TimeOutExceptionHandler(ILogger<TimeOutExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "A timeout occurred");

        if (exception is TimeoutException)
        {

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.RequestTimeout,
                Type = exception.GetType().Name,
                Title = "A timeout occurred",
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