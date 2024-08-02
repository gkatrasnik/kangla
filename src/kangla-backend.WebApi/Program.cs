using Serilog;
using Infrastructure.Services;
using Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up!");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var env = builder.Environment;

    builder.Services.AddCustomLogging(builder.Configuration)
        .AddCustomExceptionHandlers()
        .AddCustomSwagger(env)
        .AddCustomServices(builder.Configuration)
        .AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        })
        .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.ConfigureCustomInvalidModelStateResponse();
            });

    var app = builder.Build();

    app.UseCustomMiddleware(env);
    app.UseCors("AllowAllOrigins");

    // Apply migrations and seed 
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var migrationService = services.GetRequiredService<IDatabaseMigrationService>();
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            migrationService.MigrateDatabase();
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database migration or seeding.");
            throw;
        }
    }

    app.MapControllers();
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
