namespace SistemaAcademico.Models;

public class ReporteMatricula
{
    public int IdMatricula { get; set; }
    public string? Documento { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Grado { get; set; }
    public string? Seccion { get; set; }
    public string? CicloEscolar { get; set; }
    public DateTime FechaMatricula { get; set; }
    public string? Estado { get; set; }
}
