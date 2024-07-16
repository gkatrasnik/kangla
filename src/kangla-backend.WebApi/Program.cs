using Infrastructure;
using Application;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

builder.Services.AddControllers();

var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();

builder.Services.AddInfrastructureServices(builder.Configuration, logger);
builder.Services.AddApplicationServices(logger);

if (env.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();


// Apply migrations and seed 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var migrationService = services.GetRequiredService<IDatabaseMigrationService>();
    var seeder = services.GetRequiredService<DatabaseSeeder>();

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