using Carter;
using SistemaAcademico.Config;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddCarter();
builder.Services.AddCors();
builder.Services.AddSingleton<DatabaseConfig>();

var app = builder.Build();

// Middleware
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapCarter();

app.Run();