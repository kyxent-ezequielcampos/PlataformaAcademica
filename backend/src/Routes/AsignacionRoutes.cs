namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class AsignacionRoutes : CarterModule
{
    public AsignacionRoutes() : base("/api/asignaciones") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new AsignacionRepository(dbConfig);
            var controller = new AsignacionController(repository);
            var asignaciones = await controller.GetAll();
            return Results.Ok(asignaciones);
        });

        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new AsignacionRepository(dbConfig);
            var controller = new AsignacionController(repository);
            var asignacion = await controller.GetById(id);
            
            return asignacion != null ? Results.Ok(asignacion) : Results.NotFound();
        });

        app.MapPost("/", async (CreateAsignacionDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new AsignacionRepository(dbConfig);
                var controller = new AsignacionController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/asignaciones/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        app.MapPut("/{id:int}", async (int id, CreateAsignacionDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new AsignacionRepository(dbConfig);
                var controller = new AsignacionController(repository);
                var resultado = await controller.Update(id, dto);
                return Results.Ok(new { success = resultado });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        app.MapDelete("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new AsignacionRepository(dbConfig);
            var controller = new AsignacionController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });
    }
}
