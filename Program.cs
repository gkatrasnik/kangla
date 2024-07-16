using kangla_backend.Mappings;
using Microsoft.EntityFrameworkCore;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

//configure services
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();

if (env.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddInfrastructureServices(builder.Configuration, logger, env.IsDevelopment());
var app = builder.Build();



// Apply migrations and seed 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<WateringContext>();

    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }

    if (env.IsDevelopment())
    {
        try
        {
            var seeder = services.GetRequiredService<DatabaseSeeder>();
            seeder.Seed();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}





// Configure the HTTP request pipeline.


//configure middlewares
if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();






app.MapControllers();

app.Run();