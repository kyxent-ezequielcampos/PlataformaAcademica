namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class EstudianteRepository
{
    private readonly DatabaseConfig _dbConfig;

    public EstudianteRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Estudiante>> GetAll()
    {
        var estudiantes = new List<Estudiante>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT id_estudiante, documento, nombres, apellidos, fecha_nacimiento, 
                     direccion, telefono, email, foto_url, activo, fecha_registro 
              FROM estudiantes WHERE activo = true",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            estudiantes.Add(MapEstudiante(reader));
        }

        return estudiantes;
    }

    public async Task<Estudiante?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT id_estudiante, documento, nombres, apellidos, fecha_nacimiento, 
                     direccion, telefono, email, foto_url, activo, fecha_registro 
              FROM estudiantes WHERE id_estudiante = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapEstudiante(reader);
        }

        return null;
    }

    public async Task<int> Create(CreateEstudianteDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"INSERT INTO estudiantes (documento, nombres, apellidos, fecha_nacimiento, 
                                       direccion, telefono, email, foto_url) 
              VALUES (@doc, @nombres, @apellidos, @fecha, @dir, @tel, @email, @foto) 
              RETURNING id_estudiante",
            conn);
        
        cmd.Parameters.AddWithValue("doc", dto.Documento);
        cmd.Parameters.AddWithValue("nombres", dto.Nombres);
        cmd.Parameters.AddWithValue("apellidos", dto.Apellidos);
        cmd.Parameters.AddWithValue("fecha", dto.FechaNacimiento);
        cmd.Parameters.AddWithValue("dir", (object?)dto.Direccion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("tel", (object?)dto.Telefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("email", (object?)dto.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("foto", (object?)dto.FotoUrl ?? DBNull.Value);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Update(int id, CreateEstudianteDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"UPDATE estudiantes SET documento = @doc, nombres = @nombres, 
                                     apellidos = @apellidos, fecha_nacimiento = @fecha,
                                     direccion = @dir, telefono = @tel, email = @email, 
                                     foto_url = @foto 
              WHERE id_estudiante = @id",
            conn);
        
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("doc", dto.Documento);
        cmd.Parameters.AddWithValue("nombres", dto.Nombres);
        cmd.Parameters.AddWithValue("apellidos", dto.Apellidos);
        cmd.Parameters.AddWithValue("fecha", dto.FechaNacimiento);
        cmd.Parameters.AddWithValue("dir", (object?)dto.Direccion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("tel", (object?)dto.Telefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("email", (object?)dto.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("foto", (object?)dto.FotoUrl ?? DBNull.Value);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "UPDATE estudiantes SET activo = false WHERE id_estudiante = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private Estudiante MapEstudiante(NpgsqlDataReader reader)
    {
        return new Estudiante
        {
            IdEstudiante = reader.GetInt32(0),
            Documento = reader.GetString(1),
            Nombres = reader.GetString(2),
            Apellidos = reader.GetString(3),
            FechaNacimiento = reader.GetDateTime(4),
            Direccion = reader.IsDBNull(5) ? null : reader.GetString(5),
            Telefono = reader.IsDBNull(6) ? null : reader.GetString(6),
            Email = reader.IsDBNull(7) ? null : reader.GetString(7),
            FotoUrl = reader.IsDBNull(8) ? null : reader.GetString(8),
            Activo = reader.GetBoolean(9),
            FechaRegistro = reader.GetDateTime(10)
        };
    }
}