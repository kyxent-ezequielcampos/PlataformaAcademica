namespace SistemaAcademico.Routes;

using Carter;
using SistemaAcademico.Config;
using SistemaAcademico.Controllers;
using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class UsuarioRoutes : CarterModule
{
    public UsuarioRoutes() : base("/api/usuarios") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET: Obtener todos los usuarios
        app.MapGet("/", async (DatabaseConfig dbConfig) =>
        {
            var repository = new UsuarioRepository(dbConfig);
            var controller = new UsuarioController(repository);
            var usuarios = await controller.GetAll();
            return Results.Ok(usuarios);
        });

        // GET: Obtener usuario por ID
        app.MapGet("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new UsuarioRepository(dbConfig);
            var controller = new UsuarioController(repository);
            var usuario = await controller.GetById(id);
            
            return usuario != null ? Results.Ok(usuario) : Results.NotFound();
        });

        // POST: Crear usuario
        app.MapPost("/", async (CreateUsuarioDto dto, DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new UsuarioRepository(dbConfig);
                var controller = new UsuarioController(repository);
                var id = await controller.Create(dto);
                return Results.Created($"/api/usuarios/{id}", new { id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST: Login
        app.MapPost("/login", async (LoginDto dto, DatabaseConfig dbConfig) =>
        {
            var repository = new UsuarioRepository(dbConfig);
            var controller = new UsuarioController(repository);
            var usuario = await controller.Login(dto);
            
            if (usuario == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(usuario);
        });

        // POST: Crear usuario por defecto (solo para desarrollo)
        app.MapPost("/create-default", async (DatabaseConfig dbConfig) =>
        {
            try
            {
                var repository = new UsuarioRepository(dbConfig);
                var controller = new UsuarioController(repository);
                
                // Crear usuario admin por defecto
                var adminDto = new CreateUsuarioDto
                {
                    NombreUsuario = "admin",
                    Contrasena = "admin123",
                    Rol = "administrador"
                };
                
                var adminId = await controller.Create(adminDto);
                
                // Crear usuario ezequielcampos
                var userDto = new CreateUsuarioDto
                {
                    NombreUsuario = "ezequielcampos",
                    Contrasena = "ezequiel123",
                    Rol = "administrador"
                };
                
                var userId = await controller.Create(userDto);
                
                return Results.Ok(new { 
                    message = "Usuarios creados", 
                    adminId, 
                    userId 
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // DELETE: Eliminar usuario (soft delete)
        app.MapDelete("/{id:int}", async (int id, DatabaseConfig dbConfig) =>
        {
            var repository = new UsuarioRepository(dbConfig);
            var controller = new UsuarioController(repository);
            var resultado = await controller.Delete(id);
            
            return resultado ? Results.Ok() : Results.NotFound();
        });
    }
}