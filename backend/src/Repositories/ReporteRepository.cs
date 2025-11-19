using Npgsql;
using SistemaAcademico.Config;
using SistemaAcademico.Models;

namespace SistemaAcademico.Repositories;

public class ReporteRepository
{
    private readonly DatabaseConfig _dbConfig;

    public ReporteRepository(DatabaseConfig dbConfig)
    {
        _dbConfig = dbConfig;
    }

    public async Task<ReporteNotas?> ObtenerReporteNotasEstudiante(int idEstudiante, string cicloEscolar)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        // Obtener información del estudiante
        var queryEstudiante = @"
            SELECT 
                e.id_estudiante,
                e.documento,
                CONCAT(e.nombres, ' ', e.apellidos) AS nombre_completo,
                g.nombre AS grado,
                g.seccion,
                m.ciclo_escolar
            FROM estudiantes e
            INNER JOIN matriculas m ON e.id_estudiante = m.id_estudiante
            INNER JOIN grados g ON m.id_grado = g.id_grado
            WHERE e.id_estudiante = @idEstudiante 
            AND m.ciclo_escolar = @cicloEscolar
            AND m.estado = 'activo'";

        ReporteNotas? reporte = null;

        using (var cmd = new NpgsqlCommand(queryEstudiante, conn))
        {
            cmd.Parameters.AddWithValue("idEstudiante", idEstudiante);
            cmd.Parameters.AddWithValue("cicloEscolar", cicloEscolar);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                reporte = new ReporteNotas
                {
                    IdEstudiante = reader.GetInt32(0),
                    Documento = reader.GetString(1),
                    NombreCompleto = reader.GetString(2),
                    Grado = reader.GetString(3),
                    Seccion = reader.GetString(4),
                    CicloEscolar = reader.GetString(5)
                };
            }
        }

        if (reporte == null) return null;

        // Obtener calificaciones
        var queryNotas = @"
            SELECT 
                a.nombre AS asignatura,
                c.periodo,
                c.nota,
                CASE 
                    WHEN c.nota >= 60 THEN 'Aprobado'
                    ELSE 'Reprobado'
                END AS estado
            FROM calificaciones c
            INNER JOIN inscripciones i ON c.id_inscripcion = i.id_inscripcion
            INNER JOIN asignaturas a ON i.id_asignatura = a.id_asignatura
            INNER JOIN matriculas m ON i.id_matricula = m.id_matricula
            WHERE m.id_estudiante = @idEstudiante 
            AND m.ciclo_escolar = @cicloEscolar
            ORDER BY a.nombre, c.periodo";

        using (var cmd = new NpgsqlCommand(queryNotas, conn))
        {
            cmd.Parameters.AddWithValue("idEstudiante", idEstudiante);
            cmd.Parameters.AddWithValue("cicloEscolar", cicloEscolar);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                reporte.Notas.Add(new NotaAsignatura
                {
                    Asignatura = reader.GetString(0),
                    Periodo = reader.GetString(1),
                    Nota = reader.GetDecimal(2),
                    Estado = reader.GetString(3)
                });
            }
        }

        // Calcular promedio
        if (reporte.Notas.Any())
        {
            reporte.PromedioGeneral = reporte.Notas.Average(n => n.Nota);
        }

        return reporte;
    }

    public async Task<List<ReporteMatricula>> ObtenerListadoMatriculas(string cicloEscolar, int? idGrado = null)
    {
        using var conn = _dbConfig.GetConnection();
        await conn.OpenAsync();

        var query = @"
            SELECT 
                m.id_matricula,
                e.documento,
                CONCAT(e.nombres, ' ', e.apellidos) AS nombre_completo,
                e.email,
                e.telefono,
                g.nombre AS grado,
                g.seccion,
                m.ciclo_escolar,
                m.fecha_matricula,
                m.estado
            FROM matriculas m
            INNER JOIN estudiantes e ON m.id_estudiante = e.id_estudiante
            INNER JOIN grados g ON m.id_grado = g.id_grado
            WHERE m.ciclo_escolar = @cicloEscolar";

        if (idGrado.HasValue)
        {
            query += " AND m.id_grado = @idGrado";
        }

        query += " ORDER BY g.nombre, g.seccion, e.apellidos, e.nombres";

        var matriculas = new List<ReporteMatricula>();

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("cicloEscolar", cicloEscolar);
        if (idGrado.HasValue)
        {
            cmd.Parameters.AddWithValue("idGrado", idGrado.Value);
        }

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            matriculas.Add(new ReporteMatricula
            {
                IdMatricula = reader.GetInt32(0),
                Documento = reader.GetString(1),
                NombreCompleto = reader.GetString(2),
                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                Grado = reader.GetString(5),
                Seccion = reader.GetString(6),
                CicloEscolar = reader.GetString(7),
                FechaMatricula = reader.GetDateTime(8),
                Estado = reader.GetString(9)
            });
        }

        return matriculas;
    }
}
