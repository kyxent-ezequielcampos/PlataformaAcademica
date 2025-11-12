namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class CalificacionRoutes : CarterModule
{
    public CalificacionRoutes() : base("/api/calificaciones") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new CalificacionRepository(dbConfig);
            var controller = new CalificacionController(repository);
            var calificaciones = await controller.GetAll();
            return Results.Ok(calificaciones);
        });

        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new CalificacionRepository(dbConfig);
            var controller = new CalificacionController(repository);
            var calificacion = await controller.GetById(id);
            
            return calificacion != null ? Results.Ok(calificacion) : Results.NotFound();
        });

        app.MapGet("/inscripcion/{idInscripcion:int}", async (int idInscripcion, DatabaseConfig dbConfig) =>
        {
            var repository = new CalificacionRepository(dbConfig);
            var controller = new CalificacionController(repository);
            var calificaciones = await controller.GetByInscripcion(idInscripcion);
            return Results.Ok(calificaciones);
        });

        app.MapPost("/", async (CreateCalificacionDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new CalificacionRepository(dbConfig);
                var controller = new CalificacionController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/calificaciones/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        app.MapPut("/{id:int}", async (int id, CreateCalificacionDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new CalificacionRepository(dbConfig);
                var controller = new CalificacionController(repository);
                var resultado = await controller.Update(id, dto);
                return Results.Ok(new { success = resultado });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        app.MapDelete("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new CalificacionRepository(dbConfig);
            var controller = new CalificacionController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });

        // GET: Calcular promedio de estudiante usando función de PostgreSQL
        app.MapGet("/promedio/{idEstudiante:int}/{cicloEscolar}", async (int idEstudiante, string cicloEscolar, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new CalificacionRepository(dbConfig);
                var controller = new CalificacionController(repository);
                var promedio = await controller.CalcularPromedioEstudiante(idEstudiante, cicloEscolar);
                return Results.Ok(new { idEstudiante, cicloEscolar, promedio });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}
