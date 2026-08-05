using Serilog;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.RateLimiting;
using System.Security.Cryptography;
using System.Text;
using kangla.Infrastructure;
using kangla.WebApi.ExceptionHandlers;
using kangla.Application;
using kangla.Application.ClientUpdates;
using kangla.WebApi.ClientUpdates;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace kangla.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices();
            services.AddSingleton<IClientStateChangeNotifier, SignalRClientStateChangeNotifier>();
            return services;
        }

        public static IServiceCollection AddClientUpdates(this IServiceCollection services)
        {
            services.AddSignalR()
                .AddJsonProtocol(options => options.PayloadSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddIdentityApiEndpoints<IdentityUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 4;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<PlantsContext>();

            return services;
        }

        public static IServiceCollection AddSecurityRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy("identity", context => CreateFixedWindowLimiter(context, permitLimit: 20, TimeSpan.FromMinutes(1)));
                options.AddPolicy("device-api", context => CreateDeviceLimiter(context));
            });

            return services;
        }

        private static RateLimitPartition<string> CreateFixedWindowLimiter(HttpContext context, int permitLimit, TimeSpan window)
        {
            var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueLimit = 0,
                AutoReplenishment = true
            });
        }

        private static RateLimitPartition<string> CreateDeviceLimiter(HttpContext context)
        {
            var accessKey = context.Request.Headers["X-Device-Access-Key"].FirstOrDefault();
            var partitionKey = string.IsNullOrWhiteSpace(accessKey)
                ? context.Connection.RemoteIpAddress?.ToString() ?? "unknown"
                : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(accessKey)));

            return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
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
}
