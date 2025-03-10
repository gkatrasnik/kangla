using Kangla.WebApi.Middlewares;
using Serilog;

namespace kangla.WebApi.Extensions
{
    public static class WebApplicatinExtensions
    {
        public static void UseCustomMiddlewares(this IApplicationBuilder app, IHostEnvironment env)
        {
            //app.UseExceptionHandler(); We use custom exception handlers
            //app.UseStatusCodePages();
            app.UseMiddleware<SerilogEnrichmentMiddleware>();
            app.UseSerilogRequestLogging();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseAuthorization();
        }
    }
}