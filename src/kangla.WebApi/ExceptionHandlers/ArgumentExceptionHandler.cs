using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace kangla.WebApi.ExceptionHandlers
{
    public class ArgumentExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ArgumentExceptionHandler> _logger;
        public ArgumentExceptionHandler(ILogger<ArgumentExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {

            if (exception is not ArgumentException argumentException)
            {
                return false;
            }

            _logger.LogError(argumentException, "Argument exception occurred");

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = argumentException.GetType().Name,
                Title = "Argument exception occurred",
                Detail = argumentException.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;

        }
    }
}