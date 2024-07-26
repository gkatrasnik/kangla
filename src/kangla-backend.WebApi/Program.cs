using Infrastructure;
using Application;
using Infrastructure.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up!");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var env = builder.Environment;
    
    builder.Services.AddSerilog((services, lc) => lc
      .ReadFrom.Configuration(builder.Configuration)
      .ReadFrom.Services(services)
      .Enrich.FromLogContext()
      .WriteTo.Console());

    builder.Services.AddControllers();

    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApplicationServices();

    if (env.IsDevelopment())
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    var app = builder.Build();

    // Write streamlined request completion events, instead of the more verbose ones from the framework.
    // To use the default framework request logging instead, remove this line and set the "Microsoft"
    // level in appsettings.json to "Information".
    app.UseSerilogRequestLogging();
    
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

    if (env.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();

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