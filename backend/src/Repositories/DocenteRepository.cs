namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class DocenteRepository
{
    private readonly DatabaseConfig _dbConfig;

    public DocenteRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Docente>> GetAll()
    {
        var docentes = new List<Docente>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT id_docente, documento, nombres, apellidos, especialidad, 
                     telefono, email, foto_url, id_usuario, activo, fecha_registro 
              FROM docentes WHERE activo = true",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            docentes.Add(MapDocente(reader));
        }

        return docentes;
    }

    public async Task<Docente?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT id_docente, documento, nombres, apellidos, especialidad, 
                     telefono, email, foto_url, id_usuario, activo, fecha_registro 
              FROM docentes WHERE id_docente = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapDocente(reader);
        }

        return null;
    }

    public async Task<int> Create(CreateDocenteDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"INSERT INTO docentes (documento, nombres, apellidos, especialidad, 
                                    telefono, email, foto_url, id_usuario) 
              VALUES (@doc, @nombres, @apellidos, @esp, @tel, @email, @foto, @usuario) 
              RETURNING id_docente",
            conn);
        
        cmd.Parameters.AddWithValue("doc", dto.Documento);
        cmd.Parameters.AddWithValue("nombres", dto.Nombres);
        cmd.Parameters.AddWithValue("apellidos", dto.Apellidos);
        cmd.Parameters.AddWithValue("esp", (object?)dto.Especialidad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("tel", (object?)dto.Telefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("email", (object?)dto.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("foto", (object?)dto.FotoUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("usuario", (object?)dto.IdUsuario ?? DBNull.Value);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Update(int id, CreateDocenteDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"UPDATE docentes SET documento = @doc, nombres = @nombres, 
                                  apellidos = @apellidos, especialidad = @esp,
                                  telefono = @tel, email = @email, 
                                  foto_url = @foto, id_usuario = @usuario 
              WHERE id_docente = @id",
            conn);
        
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("doc", dto.Documento);
        cmd.Parameters.AddWithValue("nombres", dto.Nombres);
        cmd.Parameters.AddWithValue("apellidos", dto.Apellidos);
        cmd.Parameters.AddWithValue("esp", (object?)dto.Especialidad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("tel", (object?)dto.Telefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("email", (object?)dto.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("foto", (object?)dto.FotoUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("usuario", (object?)dto.IdUsuario ?? DBNull.Value);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "UPDATE docentes SET activo = false WHERE id_docente = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private Docente MapDocente(NpgsqlDataReader reader)
    {
        return new Docente
        {
            IdDocente = reader.GetInt32(0),
            Documento = reader.GetString(1),
            Nombres = reader.GetString(2),
            Apellidos = reader.GetString(3),
            Especialidad = reader.IsDBNull(4) ? null : reader.GetString(4),
            Telefono = reader.IsDBNull(5) ? null : reader.GetString(5),
            Email = reader.IsDBNull(6) ? null : reader.GetString(6),
            FotoUrl = reader.IsDBNull(7) ? null : reader.GetString(7),
            IdUsuario = reader.IsDBNull(8) ? null : reader.GetInt32(8),
            Activo = reader.GetBoolean(9),
            FechaRegistro = reader.GetDateTime(10)
        };
    }
}