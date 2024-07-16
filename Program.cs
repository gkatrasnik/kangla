using kangla_backend.Mappings;
using kangla_backend.Model;
using kangla_backend.Utilities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

//configure services
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<WateringContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WateringContextSQLite")));

if (env.IsDevelopment())
{
    builder.Services.AddTransient<JsonFileLoader>();
    builder.Services.AddTransient<DatabaseSeeder>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}





var app = builder.Build();



// Apply migrations and seed 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<WateringContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

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