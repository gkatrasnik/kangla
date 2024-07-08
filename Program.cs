using kangla_backend.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Apply migrations and ensure the database is created
app.ApplyMigrations();

// Configure the HTTP request pipeline.
app.ConfigureMiddlewares();

app.MapControllers();

app.Run();