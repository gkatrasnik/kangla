using Serilog;
using Microsoft.AspNetCore.Identity;
using kangla.Infrastructure.Services;
using kangla.Infrastructure;
using kangla.WebApi.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up!");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var env = builder.Environment;
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

    builder.Services.AddCustomLogging(builder.Configuration)
        .AddCustomExceptionHandlers()
        .AddCustomSwagger(env)
        .AddSecurityRateLimiting()
        .AddCustomServices(builder.Configuration)
        .AddIdentityServices()
        .AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
                });
        })
        .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)))
            .ConfigureApiBehaviorOptions(options =>
            {
                options.ConfigureCustomInvalidModelStateResponse();
            });

    var app = builder.Build();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseCustomMiddlewares(env);

    var apiGroup = app.MapGroup("/api");
    apiGroup.MapIdentityApi<IdentityUser>().RequireRateLimiting("identity");
    // logout endpoint is not implemented by asp.net core identity
    apiGroup.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
    {
        await signInManager.SignOutAsync().ConfigureAwait(false);
    });

    // Apply migrations and seed development demo data.
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var migrationService = services.GetRequiredService<IDatabaseMigrationService>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            // Ensure the directory exists before migration
            var dataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }

            migrationService.MigrateDatabase();
            if (env.IsDevelopment())
            {
                var seeder = services.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database migration or seeding.");
            throw;
        }
    }

    app.MapControllers();

    app.MapFallbackToFile("/index.html");

    app.Run();
    Log.Information("Stopped cleanly");
    return 0;
}
catch (Exception ex) when (ex is not HostAbortedException && ex.Source != "Microsoft.EntityFrameworkCore.Design") // see https://github.com/dotnet/efcore/issues/29923
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
