using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
          this IServiceCollection services,
          ILogger logger)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IWateringDeviceService, WateringDeviceService>();
            logger.LogInformation("{Project} services registered", "Application");

            return services;
        }
    }
}
