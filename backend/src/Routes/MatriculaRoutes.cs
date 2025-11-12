namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class MatriculaRoutes : CarterModule
{
    public MatriculaRoutes() : base("/api/matriculas") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new MatriculaRepository(dbConfig);
            var controller = new MatriculaController(repository);
            var matriculas = await controller.GetAll();
            return Results.Ok(matriculas);
        });

        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new MatriculaRepository(dbConfig);
            var controller = new MatriculaController(repository);
            var matricula = await controller.GetById(id);
            
            return matricula != null ? Results.Ok(matricula) : Results.NotFound();
        });

        app.MapPost("/", async (CreateMatriculaDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new MatriculaRepository(dbConfig);
                var controller = new MatriculaController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/matriculas/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        app.MapPut("/{id:int}", async (int id, CreateMatriculaDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new MatriculaRepository(dbConfig);
                var controller = new MatriculaController(repository);
                var resultado = await controller.Update(id, dto);
                return Results.Ok(new { success = resultado });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        app.MapPatch("/{id:int}/estado", async (int id, DatabaseConfig dbConfig, HttpContext context) =>
        {
            try
            {
                var body = await context.Request.ReadFromJsonAsync<Dictionary<string, string>>();
                var estado = body?["estado"] ?? "activo";
                
                var repository = new MatriculaRepository(dbConfig);
                var controller = new MatriculaController(repository);
                var resultado = await controller.UpdateEstado(id, estado);
                return Results.Ok(new { success = resultado });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        app.MapDelete("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new MatriculaRepository(dbConfig);
            var controller = new MatriculaController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });
    }
}
