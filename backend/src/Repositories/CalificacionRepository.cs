namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class CalificacionRepository
{
    private readonly DatabaseConfig _dbConfig;

    public CalificacionRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Calificacion>> GetAll()
    {
        var calificaciones = new List<Calificacion>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT c.id_calificacion, c.id_inscripcion, c.periodo, c.nota, 
                     c.fecha_registro, c.observaciones,
                     CONCAT(e.nombres, ' ', e.apellidos) as nombre_estudiante,
                     a.nombre as nombre_asignatura
              FROM calificaciones c
              INNER JOIN inscripciones i ON c.id_inscripcion = i.id_inscripcion
              INNER JOIN matriculas m ON i.id_matricula = m.id_matricula
              INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
              INNER JOIN asignaturas a ON i.id_asignatura = a.id_asignatura",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            calificaciones.Add(MapCalificacion(reader));
        }

        return calificaciones;
    }

    public async Task<Calificacion?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT c.id_calificacion, c.id_inscripcion, c.periodo, c.nota, 
                     c.fecha_registro, c.observaciones,
                     CONCAT(e.nombres, ' ', e.apellidos) as nombre_estudiante,
                     a.nombre as nombre_asignatura
              FROM calificaciones c
              INNER JOIN inscripciones i ON c.id_inscripcion = i.id_inscripcion
              INNER JOIN matriculas m ON i.id_matricula = m.id_matricula
              INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
              INNER JOIN asignaturas a ON i.id_asignatura = a.id_asignatura
              WHERE c.id_calificacion = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapCalificacion(reader);
        }

        return null;
    }

    public async Task<List<Calificacion>> GetByInscripcion(int idInscripcion)
    {
        var calificaciones = new List<Calificacion>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT c.id_calificacion, c.id_inscripcion, c.periodo, c.nota, 
                     c.fecha_registro, c.observaciones,
                     CONCAT(e.nombres, ' ', e.apellidos) as nombre_estudiante,
                     a.nombre as nombre_asignatura
              FROM calificaciones c
              INNER JOIN inscripciones i ON c.id_inscripcion = i.id_inscripcion
              INNER JOIN matriculas m ON i.id_matricula = m.id_matricula
              INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
              INNER JOIN asignaturas a ON i.id_asignatura = a.id_asignatura
              WHERE c.id_inscripcion = @idInscripcion",
            conn);
        cmd.Parameters.AddWithValue("idInscripcion", idInscripcion);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            calificaciones.Add(MapCalificacion(reader));
        }

        return calificaciones;
    }

    public async Task<int> Create(CreateCalificacionDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"INSERT INTO calificaciones (id_inscripcion, periodo, nota, observaciones) 
              VALUES (@inscripcion, @periodo, @nota, @obs) 
              RETURNING id_calificacion",
            conn);
        
        cmd.Parameters.AddWithValue("inscripcion", dto.IdInscripcion);
        cmd.Parameters.AddWithValue("periodo", dto.Periodo);
        cmd.Parameters.AddWithValue("nota", dto.Nota);
        cmd.Parameters.AddWithValue("obs", (object?)dto.Observaciones ?? DBNull.Value);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Update(int id, CreateCalificacionDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"UPDATE calificaciones SET id_inscripcion = @inscripcion, periodo = @periodo, 
                                        nota = @nota, observaciones = @obs 
              WHERE id_calificacion = @id",
            conn);
        
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("inscripcion", dto.IdInscripcion);
        cmd.Parameters.AddWithValue("periodo", dto.Periodo);
        cmd.Parameters.AddWithValue("nota", dto.Nota);
        cmd.Parameters.AddWithValue("obs", (object?)dto.Observaciones ?? DBNull.Value);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "DELETE FROM calificaciones WHERE id_calificacion = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private Calificacion MapCalificacion(NpgsqlDataReader reader)
    {
        return new Calificacion
        {
            IdCalificacion = reader.GetInt32(0),
            IdInscripcion = reader.GetInt32(1),
            Periodo = reader.GetString(2),
            Nota = reader.GetDecimal(3),
            FechaRegistro = reader.GetDateTime(4),
            Observaciones = reader.IsDBNull(5) ? null : reader.GetString(5),
            NombreEstudiante = reader.IsDBNull(6) ? null : reader.GetString(6),
            NombreAsignatura = reader.IsDBNull(7) ? null : reader.GetString(7)
        };
    }

    public async Task<decimal> CalcularPromedioEstudiante(int idEstudiante, string cicloEscolar)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT calcular_promedio_estudiante(@idEstudiante, @cicloEscolar)",
            conn);
        cmd.Parameters.AddWithValue("idEstudiante", idEstudiante);
        cmd.Parameters.AddWithValue("cicloEscolar", cicloEscolar);

        var result = await cmd.ExecuteScalarAsync();
        return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
    }
}
