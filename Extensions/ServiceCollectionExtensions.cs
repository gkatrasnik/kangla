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
            services.AddControllers();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddDbContext<WateringContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("WateringContextSQLite")));
           
            if (env.IsDevelopment())
            {
                services.AddTransient<JsonFileLoader>();
                services.AddTransient<DatabaseSeeder>();
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();
            }            

            return services;
        }
    }
}