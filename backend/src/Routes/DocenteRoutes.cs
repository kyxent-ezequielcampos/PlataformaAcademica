namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class DocenteRoutes : CarterModule
{
    public DocenteRoutes() : base("/api/docentes") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET: Obtener todos los docentes
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new DocenteRepository(dbConfig);
            var controller = new DocenteController(repository);
            var docentes = await controller.GetAll();
            return Results.Ok(docentes);
        });

        // GET: Obtener docente por ID
        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new DocenteRepository(dbConfig);
            var controller = new DocenteController(repository);
            var docente = await controller.GetById(id);
            
            return docente != null ? Results.Ok(docente) : Results.NotFound();
        });

        // POST: Crear docente
        app.MapPost("/", async (CreateDocenteDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new DocenteRepository(dbConfig);
                var controller = new DocenteController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/docentes/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT: Actualizar docente
        app.MapPut("/{id:int}", async (int id, CreateDocenteDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new DocenteRepository(dbConfig);
                var controller = new DocenteController(repository);
                var resultado = await controller.Update(id, dto);
                return Results.Ok(new { success = resultado });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        // DELETE: Eliminar docente (soft delete)
        app.MapDelete("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new DocenteRepository(dbConfig);
            var controller = new DocenteController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });
    }
}
