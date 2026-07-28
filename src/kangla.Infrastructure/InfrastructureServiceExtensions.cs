using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using kangla.Domain.Interfaces;
using kangla.Infrastructure.Repositories;
using kangla.Infrastructure.Services;
using kangla.Infrastructure;

namespace kangla.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            services.AddDbContext<PlantsContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("PlantsContextSQLite")));

            services.AddTransient<DatabaseSeeder>();
            services.AddTransient<IDatabaseMigrationService, DatabaseMigrationService>();
            services.AddScoped<IWateringDeviceRepository, WateringDeviceRepository>();
            services.AddScoped<IWateringEventRepository, WateringEventRepository>();
            services.AddScoped<IHumidityMeasurementRepository, HumidityMeasurementRepository>();
            services.AddScoped<IPlantsRepository, PlantsRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddResendEmail(configuration);
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IImageProcessingService, ImageProcessingService>();
            services.AddScoped<IPlantRecognitionService, PlantRecognitionService>();
            return services;
        }
    }
}
