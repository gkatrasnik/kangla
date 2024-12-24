using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
          this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IWateringDeviceService, WateringDeviceService>();
            services.AddScoped<IWateringEventService, WateringEventService>();
            services.AddScoped<IHumidityMeasurementService, HumidityMeasurementService>();
            services.AddScoped<IPlantsService, PlantsService>();
            services.AddScoped<IImageService, ImageService>();
            return services;
        }
    }
}
