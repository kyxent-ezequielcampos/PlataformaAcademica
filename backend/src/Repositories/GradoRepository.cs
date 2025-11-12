namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class GradoRepository
{
    private readonly DatabaseConfig _dbConfig;

    public GradoRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Grado>> GetAll()
    {
        var grados = new List<Grado>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT id_grado, nombre, nivel, seccion, activo FROM grados WHERE activo = true",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            grados.Add(MapGrado(reader));
        }

        return grados;
    }

    public async Task<Grado?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT id_grado, nombre, nivel, seccion, activo FROM grados WHERE id_grado = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapGrado(reader);
        }

        return null;
    }

    public async Task<int> Create(CreateGradoDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"INSERT INTO grados (nombre, nivel, seccion) 
              VALUES (@nombre, @nivel, @seccion) 
              RETURNING id_grado",
            conn);
        
        cmd.Parameters.AddWithValue("nombre", dto.Nombre);
        cmd.Parameters.AddWithValue("nivel", (object?)dto.Nivel ?? DBNull.Value);
        cmd.Parameters.AddWithValue("seccion", (object?)dto.Seccion ?? DBNull.Value);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Update(int id, CreateGradoDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"UPDATE grados SET nombre = @nombre, nivel = @nivel, seccion = @seccion 
              WHERE id_grado = @id",
            conn);
        
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("nombre", dto.Nombre);
        cmd.Parameters.AddWithValue("nivel", (object?)dto.Nivel ?? DBNull.Value);
        cmd.Parameters.AddWithValue("seccion", (object?)dto.Seccion ?? DBNull.Value);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "UPDATE grados SET activo = false WHERE id_grado = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private Grado MapGrado(NpgsqlDataReader reader)
    {
        return new Grado
        {
            IdGrado = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Nivel = reader.IsDBNull(2) ? null : reader.GetString(2),
            Seccion = reader.IsDBNull(3) ? null : reader.GetString(3),
            Activo = reader.GetBoolean(4)
        };
    }
}
