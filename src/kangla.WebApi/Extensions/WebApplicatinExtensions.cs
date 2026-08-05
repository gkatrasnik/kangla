using Kangla.WebApi.Middlewares;
using Serilog;

namespace kangla.WebApi.Extensions
{
    public static class WebApplicatinExtensions
    {
        public static void UseCustomMiddlewares(this IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseExceptionHandler();
            //app.UseStatusCodePages();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hubs/client-updates")
                    && context.Request.Query.TryGetValue("access_token", out var accessToken)
                    && !string.IsNullOrWhiteSpace(accessToken))
                {
                    context.Request.Headers.Authorization = $"Bearer {accessToken}";
                }

                await next();
            });
            app.UseMiddleware<SerilogEnrichmentMiddleware>();
            app.UseSerilogRequestLogging();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
