using kangla_backend.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

var app = builder.Build();

// Apply migrations and seed if development
app.ApplyMigrationsAndSeed();

// Configure the HTTP request pipeline.
app.ConfigureMiddlewares();

app.MapControllers();

app.Run();