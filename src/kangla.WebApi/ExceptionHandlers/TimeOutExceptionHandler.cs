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
        if (exception is not TimeoutException timeoutException)
        {
            return false;
        }

        _logger.LogError(timeoutException, "A timeout occurred");

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.RequestTimeout,
            Type = timeoutException.GetType().Name,
            Title = "A timeout occurred",
            Detail = timeoutException.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}