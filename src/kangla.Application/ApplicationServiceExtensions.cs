using kangla.Application.HumidityMeasurements;
using kangla.Application.Images;
using kangla.Application.Plants;
using kangla.Application.Shared;
using kangla.Application.WateringDevices;
using kangla.Application.WateringEvents;
using Microsoft.Extensions.DependencyInjection;

namespace kangla.Application
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
