namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class InscripcionRepository
{
    private readonly DatabaseConfig _dbConfig;

    public InscripcionRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Inscripcion>> GetAll()
    {
        var inscripciones = new List<Inscripcion>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT i.id_inscripcion, i.id_matricula, i.id_asignatura,
                     CONCAT(e.nombres, ' ', e.apellidos) as nombre_estudiante,
                     a.nombre as nombre_asignatura
              FROM inscripciones i
              INNER JOIN matriculas m ON i.id_matricula = m.id_matricula
              INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
              INNER JOIN asignaturas a ON i.id_asignatura = a.id_asignatura",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            inscripciones.Add(MapInscripcion(reader));
        }

        return inscripciones;
    }

    public async Task<Inscripcion?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT i.id_inscripcion, i.id_matricula, i.id_asignatura,
                     CONCAT(e.nombres, ' ', e.apellidos) as nombre_estudiante,
                     a.nombre as nombre_asignatura
              FROM inscripciones i
              INNER JOIN matriculas m ON i.id_matricula = m.id_matricula
              INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
              INNER JOIN asignaturas a ON i.id_asignatura = a.id_asignatura
              WHERE i.id_inscripcion = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapInscripcion(reader);
        }

        return null;
    }

    public async Task<List<Inscripcion>> GetByMatricula(int idMatricula)
    {
        var inscripciones = new List<Inscripcion>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT i.id_inscripcion, i.id_matricula, i.id_asignatura,
                     CONCAT(e.nombres, ' ', e.apellidos) as nombre_estudiante,
                     a.nombre as nombre_asignatura
              FROM inscripciones i
              INNER JOIN matriculas m ON i.id_matricula = m.id_matricula
              INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
              INNER JOIN asignaturas a ON i.id_asignatura = a.id_asignatura
              WHERE i.id_matricula = @idMatricula",
            conn);
        cmd.Parameters.AddWithValue("idMatricula", idMatricula);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            inscripciones.Add(MapInscripcion(reader));
        }

        return inscripciones;
    }

    public async Task<int> Create(CreateInscripcionDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"INSERT INTO inscripciones (id_matricula, id_asignatura) 
              VALUES (@matricula, @asignatura) 
              RETURNING id_inscripcion",
            conn);
        
        cmd.Parameters.AddWithValue("matricula", dto.IdMatricula);
        cmd.Parameters.AddWithValue("asignatura", dto.IdAsignatura);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "DELETE FROM inscripciones WHERE id_inscripcion = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private Inscripcion MapInscripcion(NpgsqlDataReader reader)
    {
        return new Inscripcion
        {
            IdInscripcion = reader.GetInt32(0),
            IdMatricula = reader.GetInt32(1),
            IdAsignatura = reader.GetInt32(2),
            NombreEstudiante = reader.IsDBNull(3) ? null : reader.GetString(3),
            NombreAsignatura = reader.IsDBNull(4) ? null : reader.GetString(4)
        };
    }
}
