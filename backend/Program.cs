using Carter;
using SistemaAcademico.Config;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddCarter();
builder.Services.AddCors();
builder.Services.AddSingleton<DatabaseConfig>();
builder.Services.AddScoped<SistemaAcademico.Repositories.ReporteRepository>();
builder.Services.AddScoped<SistemaAcademico.Services.PdfService>();

// Agregar logging
builder.Services.AddLogging();

var app = builder.Build();

// Middleware de logging
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"🔥 {context.Request.Method} {context.Request.Path} - {DateTime.Now}");
    
    if (context.Request.Method == "POST")
    {
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        logger.LogInformation($"📦 Body: {body}");
    }
    
    await next();
    
    logger.LogInformation($"✅ Response: {context.Response.StatusCode}");
});

// Middleware
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Health check endpoint
app.MapGet("/api/health", () => 
{
    Console.WriteLine("🏥 Health check called");
    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
});

app.MapCarter();

Console.WriteLine("🚀 Backend iniciado en http://localhost:5130");
app.Run();