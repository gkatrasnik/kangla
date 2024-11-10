using Serilog;
using Infrastructure;
using Application;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddApplicationServices();
        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddAuthorization();      
        services.AddIdentityApiEndpoints<IdentityUser>(options => {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 0;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<WateringContext>();

        return services;
    }

    public static IServiceCollection AddCustomExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<InvalidOperationExceptionHandler>();
        services.AddExceptionHandler<ArgumentExceptionHandler>();
        services.AddExceptionHandler<KeyNotFoundExceptionHandler>();
        services.AddExceptionHandler<TimeOutExceptionHandler>();
        services.AddExceptionHandler<UnauthorizedAccessExceptionHandler>();
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
            services.AddSwaggerGen(options => 
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
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
