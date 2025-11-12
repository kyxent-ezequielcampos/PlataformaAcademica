namespace SistemaAcademico.Repositories;

using SistemaAcademico.Config;
using SistemaAcademico.Models;
using Npgsql;

public class AsignacionRepository
{
    private readonly DatabaseConfig _dbConfig;

    public AsignacionRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<List<Asignacion>> GetAll()
    {
        var asignaciones = new List<Asignacion>();
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT a.id_asignacion, a.id_docente, a.id_asignatura, a.id_grado, 
                     a.ciclo_escolar, a.activo,
                     CONCAT(d.nombres, ' ', d.apellidos) as nombre_docente,
                     asig.nombre as nombre_asignatura,
                     g.nombre as nombre_grado
              FROM asignaciones a
              INNER JOIN docentes d ON a.id_docente = d.id_docente
              INNER JOIN asignaturas asig ON a.id_asignatura = asig.id_asignatura
              INNER JOIN grados g ON a.id_grado = g.id_grado
              WHERE a.activo = true",
            conn);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            asignaciones.Add(MapAsignacion(reader));
        }

        return asignaciones;
    }

    public async Task<Asignacion?> GetById(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"SELECT a.id_asignacion, a.id_docente, a.id_asignatura, a.id_grado, 
                     a.ciclo_escolar, a.activo,
                     CONCAT(d.nombres, ' ', d.apellidos) as nombre_docente,
                     asig.nombre as nombre_asignatura,
                     g.nombre as nombre_grado
              FROM asignaciones a
              INNER JOIN docentes d ON a.id_docente = d.id_docente
              INNER JOIN asignaturas asig ON a.id_asignatura = asig.id_asignatura
              INNER JOIN grados g ON a.id_grado = g.id_grado
              WHERE a.id_asignacion = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapAsignacion(reader);
        }

        return null;
    }

    public async Task<int> Create(CreateAsignacionDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"INSERT INTO asignaciones (id_docente, id_asignatura, id_grado, ciclo_escolar) 
              VALUES (@docente, @asignatura, @grado, @ciclo) 
              RETURNING id_asignacion",
            conn);
        
        cmd.Parameters.AddWithValue("docente", dto.IdDocente);
        cmd.Parameters.AddWithValue("asignatura", dto.IdAsignatura);
        cmd.Parameters.AddWithValue("grado", dto.IdGrado);
        cmd.Parameters.AddWithValue("ciclo", dto.CicloEscolar);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<bool> Update(int id, CreateAsignacionDto dto)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            @"UPDATE asignaciones SET id_docente = @docente, id_asignatura = @asignatura, 
                                      id_grado = @grado, ciclo_escolar = @ciclo 
              WHERE id_asignacion = @id",
            conn);
        
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("docente", dto.IdDocente);
        cmd.Parameters.AddWithValue("asignatura", dto.IdAsignatura);
        cmd.Parameters.AddWithValue("grado", dto.IdGrado);
        cmd.Parameters.AddWithValue("ciclo", dto.CicloEscolar);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> Delete(int id)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "UPDATE asignaciones SET activo = false WHERE id_asignacion = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private Asignacion MapAsignacion(NpgsqlDataReader reader)
    {
        return new Asignacion
        {
            IdAsignacion = reader.GetInt32(0),
            IdDocente = reader.GetInt32(1),
            IdAsignatura = reader.GetInt32(2),
            IdGrado = reader.GetInt32(3),
            CicloEscolar = reader.GetString(4),
            Activo = reader.GetBoolean(5),
            NombreDocente = reader.IsDBNull(6) ? null : reader.GetString(6),
            NombreAsignatura = reader.IsDBNull(7) ? null : reader.GetString(7),
            NombreGrado = reader.IsDBNull(8) ? null : reader.GetString(8)
        };
    }
}
