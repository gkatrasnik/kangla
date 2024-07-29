using Serilog;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Infrastructure;
using Application;
using Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddApplicationServices();
        return services;
    }

    public static IServiceCollection AddCustomExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<ArgumentExceptionHandler>();
        services.AddExceptionHandler<KeyNotFoundExceptionHandler>();
        services.AddExceptionHandler<TimeOutExceptionHandler>();
        services.AddExceptionHandler<DefaultExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }

    public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());
        return services;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
        return services;
    }

    public static void UseCustomMiddleware(this IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
    }
}
