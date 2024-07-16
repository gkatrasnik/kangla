using Microsoft.EntityFrameworkCore;
using kangla_backend.Model;
using kangla_backend.Mappings;
using kangla_backend.Utilities;

namespace kangla_backend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
           

            return services;
        }
    }
}