using Microsoft.EntityFrameworkCore;
using kangla_backend.Model;
namespace kangla_backend.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureMiddlewares(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            return app;
        }

        public static void ApplyMigrationsAndSeed(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<WateringContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                var env = services.GetRequiredService<IHostEnvironment>();

                try
                {
                    context.Database.Migrate();
                    if (env == null || env.IsDevelopment())
                    {
                        var seeder = services.GetRequiredService<DatabaseSeeder>();
                        seeder.Seed();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database.");
                    throw;
                }
                               
            }
        }
    }
}
