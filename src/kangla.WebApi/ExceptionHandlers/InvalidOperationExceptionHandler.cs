using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace kangla.WebApi.ExceptionHandlers
{
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

            if (exception is not InvalidOperationException invalidOperationException)
            {
                return false;
            }

            _logger.LogError(invalidOperationException, "An invalid operation was attempted");

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = invalidOperationException.GetType().Name,
                Title = "Invalid Operation",
                Detail = invalidOperationException.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}