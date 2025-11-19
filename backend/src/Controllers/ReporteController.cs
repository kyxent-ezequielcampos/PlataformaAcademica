using Carter;
using SistemaAcademico.Repositories;
using SistemaAcademico.Services;

namespace SistemaAcademico.Controllers;

public class ReporteController : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reportes").WithTags("Reportes");

        // Reporte de notas de un estudiante
        group.MapGet("/notas/{idEstudiante}", async (int idEstudiante, string cicloEscolar, ReporteRepository repo, PdfService pdfService) =>
        {
            try
            {
                var reporte = await repo.ObtenerReporteNotasEstudiante(idEstudiante, cicloEscolar);
                
                if (reporte == null)
                {
                    return Results.NotFound(new { mensaje = "No se encontraron datos para el estudiante en el ciclo escolar especificado" });
                }

                var pdfBytes = pdfService.GenerarReporteNotas(reporte);
                
                return Results.File(
                    pdfBytes, 
                    "application/pdf", 
                    $"Reporte_Notas_{reporte.NombreCompleto?.Replace(" ", "_")}_{cicloEscolar}.pdf"
                );
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al generar reporte: {ex.Message}");
            }
        });

        // Listado de matrículas
        group.MapGet("/matriculas", async (string cicloEscolar, int? idGrado, ReporteRepository repo, PdfService pdfService) =>
        {
            try
            {
                var matriculas = await repo.ObtenerListadoMatriculas(cicloEscolar, idGrado);
                
                if (!matriculas.Any())
                {
                    return Results.NotFound(new { mensaje = "No se encontraron matrículas para los criterios especificados" });
                }

                var pdfBytes = pdfService.GenerarListadoMatriculas(matriculas, cicloEscolar);
                
                var nombreArchivo = idGrado.HasValue 
                    ? $"Listado_Matriculas_{cicloEscolar}_Grado_{idGrado}.pdf"
                    : $"Listado_Matriculas_{cicloEscolar}.pdf";
                
                return Results.File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al generar reporte: {ex.Message}");
            }
        });

        // Vista previa de datos de notas (JSON)
        group.MapGet("/notas/{idEstudiante}/preview", async (int idEstudiante, string cicloEscolar, ReporteRepository repo) =>
        {
            try
            {
                var reporte = await repo.ObtenerReporteNotasEstudiante(idEstudiante, cicloEscolar);
                
                if (reporte == null)
                {
                    return Results.NotFound(new { mensaje = "No se encontraron datos" });
                }

                return Results.Ok(reporte);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error: {ex.Message}");
            }
        });

        // Vista previa de matrículas (JSON)
        group.MapGet("/matriculas/preview", async (string cicloEscolar, int? idGrado, ReporteRepository repo) =>
        {
            try
            {
                var matriculas = await repo.ObtenerListadoMatriculas(cicloEscolar, idGrado);
                
                return Results.Ok(new 
                { 
                    total = matriculas.Count,
                    cicloEscolar,
                    idGrado,
                    matriculas 
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error: {ex.Message}");
            }
        });
    }
}
