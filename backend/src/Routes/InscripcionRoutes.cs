namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class InscripcionRoutes : CarterModule
{
    public InscripcionRoutes() : base("/api/inscripciones") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new InscripcionRepository(dbConfig);
            var controller = new InscripcionController(repository);
            var inscripciones = await controller.GetAll();
            return Results.Ok(inscripciones);
        });

        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new InscripcionRepository(dbConfig);
            var controller = new InscripcionController(repository);
            var inscripcion = await controller.GetById(id);
            
            return inscripcion != null ? Results.Ok(inscripcion) : Results.NotFound();
        });

        app.MapGet("/matricula/{idMatricula:int}", async (int idMatricula, DatabaseConfig dbConfig) =>
        {
            var repository = new InscripcionRepository(dbConfig);
            var controller = new InscripcionController(repository);
            var inscripciones = await controller.GetByMatricula(idMatricula);
            return Results.Ok(inscripciones);
        });

        app.MapPost("/", async (CreateInscripcionDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new InscripcionRepository(dbConfig);
                var controller = new InscripcionController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/inscripciones/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        app.MapDelete("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new InscripcionRepository(dbConfig);
            var controller = new InscripcionController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });
    }
}
