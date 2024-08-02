using Serilog;
using Infrastructure;
using Application;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        services.AddExceptionHandler<InvalidOperationExceptionHandler>();
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

    public static void ConfigureCustomInvalidModelStateResponse(this ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>() ?? throw new InvalidOperationException("Logger not available.");
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                });

            logger.LogWarning("Validation errors occurred: {@Errors}", errors);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "One or more validation errors occurred.",
                Detail = "Please refer to the errors property for details.",
                Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}",
                Extensions = { ["errors"] = errors }
            };

            var result = new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/json" }
            };

            return result;
        };
    }    
}
