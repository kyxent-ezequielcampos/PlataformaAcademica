namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class GradoRoutes : CarterModule
{
    public GradoRoutes() : base("/api/grados") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new GradoRepository(dbConfig);
            var controller = new GradoController(repository);
            var grados = await controller.GetAll();
            return Results.Ok(grados);
        });

        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new GradoRepository(dbConfig);
            var controller = new GradoController(repository);
            var grado = await controller.GetById(id);
            
            return grado != null ? Results.Ok(grado) : Results.NotFound();
        });

        app.MapPost("/", async (CreateGradoDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new GradoRepository(dbConfig);
                var controller = new GradoController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/grados/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        app.MapPut("/{id:int}", async (int id, CreateGradoDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new GradoRepository(dbConfig);
                var controller = new GradoController(repository);
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
            var repository = new GradoRepository(dbConfig);
            var controller = new GradoController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });
    }
}
