namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class EstudianteRoutes : CarterModule
{
    public EstudianteRoutes() : base("/api/estudiantes") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET: Obtener todos los estudiantes
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new EstudianteRepository(dbConfig);
            var controller = new EstudianteController(repository);
            var estudiantes = await controller.GetAll();
            return Results.Ok(estudiantes);
        });

        // GET: Obtener estudiante por ID
        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new EstudianteRepository(dbConfig);
            var controller = new EstudianteController(repository);
            var estudiante = await controller.GetById(id);
            
            return estudiante != null ? Results.Ok(estudiante) : Results.NotFound();
        });

        // POST: Crear estudiante
        app.MapPost("/", async (CreateEstudianteDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new EstudianteRepository(dbConfig);
                var controller = new EstudianteController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/estudiantes/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT: Actualizar estudiante
        app.MapPut("/{id:int}", async (int id, CreateEstudianteDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new EstudianteRepository(dbConfig);
                var controller = new EstudianteController(repository);
                var resultado = await controller.Update(id, dto);
                return Results.Ok(new { success = resultado });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        // DELETE: Eliminar estudiante (soft delete)
        app.MapDelete("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new EstudianteRepository(dbConfig);
            var controller = new EstudianteController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });
    }
}
