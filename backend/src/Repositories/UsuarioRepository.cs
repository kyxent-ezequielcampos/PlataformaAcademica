namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class UsuarioRepository
{
    private readonly DatabaseConfig _dbConfig;

    public UsuarioRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Usuario>> GetAll()
    {
        var usuarios = new List<Usuario>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT id_usuario, nombre_usuario, rol, activo, fecha_creacion FROM usuarios WHERE activo = true",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            usuarios.Add(new Usuario
            {
                IdUsuario = reader.GetInt32(0),
                NombreUsuario = reader.GetString(1),
                Rol = reader.GetString(2),
                Activo = reader.GetBoolean(3),
                FechaCreacion = reader.GetDateTime(4)
            });
        }

        return usuarios;
    }

    public async Task<Usuario?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT id_usuario, nombre_usuario, rol, activo, fecha_creacion FROM usuarios WHERE id_usuario = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32(0),
                NombreUsuario = reader.GetString(1),
                Rol = reader.GetString(2),
                Activo = reader.GetBoolean(3),
                FechaCreacion = reader.GetDateTime(4)
            };
        }

        return null;
    }

    public async Task<Usuario?> GetByUsername(string username)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT id_usuario, nombre_usuario, contrasena, rol, activo FROM usuarios WHERE nombre_usuario = @username",
            conn);
        cmd.Parameters.AddWithValue("username", username);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32(0),
                NombreUsuario = reader.GetString(1),
                Contrasena = reader.GetString(2),
                Rol = reader.GetString(3),
                Activo = reader.GetBoolean(4)
            };
        }

        return null;
    }

    public async Task<int> Create(CreateUsuarioDto dto, string hashedPassword)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "INSERT INTO usuarios (nombre_usuario, contrasena, rol) VALUES (@username, @password, @rol) RETURNING id_usuario",
            conn);
        cmd.Parameters.AddWithValue("username", dto.NombreUsuario);
        cmd.Parameters.AddWithValue("password", hashedPassword);
        cmd.Parameters.AddWithValue("rol", dto.Rol);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "UPDATE usuarios SET activo = false WHERE id_usuario = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}