using Application.Mappings;
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
            logger.LogInformation("{Project} services registered", "Application");

            return services;
        }
    }
}
