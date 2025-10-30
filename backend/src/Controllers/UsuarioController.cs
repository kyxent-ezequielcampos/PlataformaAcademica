namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;
using BCrypt.Net;

public class UsuarioController
{
    private readonly UsuarioRepository _repository;

    public UsuarioController(UsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Usuario>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Usuario?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<int> Create(CreateUsuarioDto dto)
    {
        // Validar rol
        var rolesValidos = new[] { "administrador", "docente", "secretario" };
        if (!rolesValidos.Contains(dto.Rol.ToLower()))
        {
            throw new ArgumentException("Rol inválido");
        }

        // Hash de la contraseña
        var hashedPassword = BCrypt.HashPassword(dto.Contrasena);
        
        return await _repository.Create(dto, hashedPassword);
    }

    public async Task<Usuario?> Login(LoginDto dto)
    {
        var usuario = await _repository.GetByUsername(dto.NombreUsuario);
        
        if (usuario == null || !usuario.Activo)
        {
            return null;
        }

        // Verificar contraseña
        if (!BCrypt.Verify(dto.Contrasena, usuario.Contrasena))
        {
            return null;
        }

        // No devolver la contraseña
        usuario.Contrasena = string.Empty;
        return usuario;
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}