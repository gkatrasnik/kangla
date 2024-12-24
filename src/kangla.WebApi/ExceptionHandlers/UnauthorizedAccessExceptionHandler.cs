using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace kangla.WebApi.ExceptionHandlers
{
    public class UnauthorizedAccessExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<UnauthorizedAccessExceptionHandler> _logger;
        public UnauthorizedAccessExceptionHandler(ILogger<UnauthorizedAccessExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not UnauthorizedAccessException unauthorizedException)
            {
                return false;
            }

            _logger.LogError(unauthorizedException, "Unauthorized access attempt");

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Type = unauthorizedException.GetType().Name,
                Title = "Unauthorized Access",
                Detail = unauthorizedException.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}