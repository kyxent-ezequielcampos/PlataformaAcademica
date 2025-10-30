namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class AsignaturaRepository
{
    private readonly DatabaseConfig _dbConfig;

    public AsignaturaRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Asignatura>> GetAll()
    {
        var asignaturas = new List<Asignatura>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT id_asignatura, codigo, nombre, descripcion, creditos, activo FROM asignaturas WHERE activo = true",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            asignaturas.Add(MapAsignatura(reader));
        }

        return asignaturas;
    }

    public async Task<Asignatura?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT id_asignatura, codigo, nombre, descripcion, creditos, activo FROM asignaturas WHERE id_asignatura = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapAsignatura(reader);
        }

        return null;
    }

    public async Task<int> Create(CreateAsignaturaDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "INSERT INTO asignaturas (codigo, nombre, descripcion, creditos) VALUES (@cod, @nom, @desc, @cred) RETURNING id_asignatura",
            conn);
        
        cmd.Parameters.AddWithValue("cod", dto.Codigo);
        cmd.Parameters.AddWithValue("nom", dto.Nombre);
        cmd.Parameters.AddWithValue("desc", (object?)dto.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cred", dto.Creditos);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Update(int id, CreateAsignaturaDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "UPDATE asignaturas SET codigo = @cod, nombre = @nom, descripcion = @desc, creditos = @cred WHERE id_asignatura = @id",
            conn);
        
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("cod", dto.Codigo);
        cmd.Parameters.AddWithValue("nom", dto.Nombre);
        cmd.Parameters.AddWithValue("desc", (object?)dto.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cred", dto.Creditos);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "UPDATE asignaturas SET activo = false WHERE id_asignatura = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private Asignatura MapAsignatura(NpgsqlDataReader reader)
    {
        return new Asignatura
        {
            IdAsignatura = reader.GetInt32(0),
            Codigo = reader.GetString(1),
            Nombre = reader.GetString(2),
            Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
            Creditos = reader.GetInt32(4),
            Activo = reader.GetBoolean(5)
        };
    }
}