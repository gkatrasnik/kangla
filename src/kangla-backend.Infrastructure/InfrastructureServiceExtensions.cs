using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            services.AddDbContext<WateringContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("WateringContextSQLite")));

            services.AddTransient<DatabaseSeeder>();                
            services.AddTransient<IDatabaseMigrationService, DatabaseMigrationService>();
            services.AddScoped<IWateringDeviceRepository, WateringDeviceRepository>();
            services.AddScoped<IWateringEventRepository, WateringEventRepository>();
            services.AddScoped<IHumidityMeasurementRepository, HumidityMeasurementRepository>();

            return services;
        }
    }
}
