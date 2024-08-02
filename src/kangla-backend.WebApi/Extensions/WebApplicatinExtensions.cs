using Serilog;

public static class WebApplicatinExtensions
{
    public static void UseCustomMiddleware(this IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
    }
}
