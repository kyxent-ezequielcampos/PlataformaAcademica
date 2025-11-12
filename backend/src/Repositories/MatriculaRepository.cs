namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class MatriculaRepository
{
    private readonly DatabaseConfig _dbConfig;

    public MatriculaRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Matricula>> GetAll()
    {
        var matriculas = new List<Matricula>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT m.id_matricula, m.id_estudiante, m.id_grado, m.ciclo_escolar, 
                     m.fecha_matricula, m.estado,
                     CONCAT(e.nombres, ' ', e.apellidos) as nombre_estudiante,
                     g.nombre as nombre_grado
              FROM matriculas m
              INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
              INNER JOIN grados g ON m.id_grado = g.id_grado",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            matriculas.Add(MapMatricula(reader));
        }

        return matriculas;
    }

    public async Task<Matricula?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT m.id_matricula, m.id_estudiante, m.id_grado, m.ciclo_escolar, 
                     m.fecha_matricula, m.estado,
                     CONCAT(e.nombres, ' ', e.apellidos) as nombre_estudiante,
                     g.nombre as nombre_grado
              FROM matriculas m
              INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
              INNER JOIN grados g ON m.id_grado = g.id_grado
              WHERE m.id_matricula = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapMatricula(reader);
        }

        return null;
    }

    public async Task<int> Create(CreateMatriculaDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"INSERT INTO matriculas (id_estudiante, id_grado, ciclo_escolar) 
              VALUES (@estudiante, @grado, @ciclo) 
              RETURNING id_matricula",
            conn);
        
        cmd.Parameters.AddWithValue("estudiante", dto.IdEstudiante);
        cmd.Parameters.AddWithValue("grado", dto.IdGrado);
        cmd.Parameters.AddWithValue("ciclo", dto.CicloEscolar);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Update(int id, CreateMatriculaDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"UPDATE matriculas SET id_estudiante = @estudiante, id_grado = @grado, 
                                    ciclo_escolar = @ciclo 
              WHERE id_matricula = @id",
            conn);
        
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("estudiante", dto.IdEstudiante);
        cmd.Parameters.AddWithValue("grado", dto.IdGrado);
        cmd.Parameters.AddWithValue("ciclo", dto.CicloEscolar);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateEstado(int id, string estado)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "UPDATE matriculas SET estado = @estado WHERE id_matricula = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("estado", estado);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "DELETE FROM matriculas WHERE id_matricula = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private Matricula MapMatricula(NpgsqlDataReader reader)
    {
        return new Matricula
        {
            IdMatricula = reader.GetInt32(0),
            IdEstudiante = reader.GetInt32(1),
            IdGrado = reader.GetInt32(2),
            CicloEscolar = reader.GetString(3),
            FechaMatricula = reader.GetDateTime(4),
            Estado = reader.GetString(5),
            NombreEstudiante = reader.IsDBNull(6) ? null : reader.GetString(6),
            NombreGrado = reader.IsDBNull(7) ? null : reader.GetString(7)
        };
    }
}
