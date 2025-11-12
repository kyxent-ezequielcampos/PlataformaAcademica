namespace SistemaAcademico.Models;

public class Matricula
{
    public int IdMatricula { get; set; }
    public int IdEstudiante { get; set; }
    public int IdGrado { get; set; }
    public string CicloEscolar { get; set; } = string.Empty;
    public DateTime FechaMatricula { get; set; }
    public string Estado { get; set; } = "activo";
    
    // Propiedades de navegación
    public string? NombreEstudiante { get; set; }
    public string? NombreGrado { get; set; }
}

public class CreateMatriculaDto
{
    public int IdEstudiante { get; set; }
    public int IdGrado { get; set; }
    public string CicloEscolar { get; set; } = string.Empty;
}
