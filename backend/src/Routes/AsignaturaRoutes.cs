namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class AsignaturaRoutes : CarterModule
{
    public AsignaturaRoutes() : base("/api/asignaturas") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET: Obtener todas las asignaturas
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new AsignaturaRepository(dbConfig);
            var controller = new AsignaturaController(repository);
            var asignaturas = await controller.GetAll();
            return Results.Ok(asignaturas);
        });

        // GET: Obtener asignatura por ID
        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new AsignaturaRepository(dbConfig);
            var controller = new AsignaturaController(repository);
            var asignatura = await controller.GetById(id);
            
            return asignatura != null ? Results.Ok(asignatura) : Results.NotFound();
        });

        // POST: Crear asignatura
        app.MapPost("/", async (CreateAsignaturaDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new AsignaturaRepository(dbConfig);
                var controller = new AsignaturaController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/asignaturas/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT: Actualizar asignatura
        app.MapPut("/{id:int}", async (int id, CreateAsignaturaDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new AsignaturaRepository(dbConfig);
                var controller = new AsignaturaController(repository);
                var resultado = await controller.Update(id, dto);
                return Results.Ok(new { success = resultado });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        // DELETE: Eliminar asignatura (soft delete)
        app.MapDelete("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new AsignaturaRepository(dbConfig);
            var controller = new AsignaturaController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });
    }
}