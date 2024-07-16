using Microsoft.EntityFrameworkCore;
using kangla_backend.Model;
namespace kangla_backend.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureMiddlewares(this IApplicationBuilder app)
        {
            

            return app;
        }

        public static void ApplyMigrationsAndSeed(this IApplicationBuilder app)
        {
            
        }
    }
}
