using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace kangla.WebApi.ExceptionHandlers
{
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

            if (exception is not KeyNotFoundException keyNotFoundException)
            {
                return false;
            }

            _logger.LogError(keyNotFoundException, "Key was not found");

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Type = keyNotFoundException.GetType().Name,
                Title = "Key was not found",
                Detail = keyNotFoundException.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}