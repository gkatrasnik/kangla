using Serilog.Context;
using System.Security.Claims;

namespace Kangla.WebApi.Middlewares
{
    public class SerilogEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;

        public SerilogEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            using (LogContext.PushProperty("IPAddress", ipAddress ?? "Unknown"))
            using (LogContext.PushProperty("UserId", userId ?? "Anonymous"))
            {
                await _next(context);
            }
        }
    }
}