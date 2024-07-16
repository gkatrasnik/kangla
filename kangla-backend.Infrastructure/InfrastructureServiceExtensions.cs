using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
          this IServiceCollection services,
          IConfiguration configuration,
          ILogger logger,
          bool isDevelopement)
        {
            services.AddDbContext<WateringContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("WateringContextSQLite")));

            services.AddScoped<IWateringDeviceRepository, WateringDeviceRepository>();

            if (isDevelopement)
            {
                services.AddTransient<DatabaseSeeder>();                
                services.AddTransient<IDatabaseMigrationService, DatabaseMigrationService>();                
            }

            logger.LogInformation("{Project} services registered", "Infrastructure");

            return services;
        }
    }
}
