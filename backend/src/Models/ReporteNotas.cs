namespace SistemaAcademico.Models;

public class ReporteNotas
{
    public int IdEstudiante { get; set; }
    public string? Documento { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Grado { get; set; }
    public string? Seccion { get; set; }
    public string? CicloEscolar { get; set; }
    public List<NotaAsignatura> Notas { get; set; } = new();
    public decimal PromedioGeneral { get; set; }
}

public class NotaAsignatura
{
    public string? Asignatura { get; set; }
    public string? Periodo { get; set; }
    public decimal Nota { get; set; }
    public string? Estado { get; set; }
}
