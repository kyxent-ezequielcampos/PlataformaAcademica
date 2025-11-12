namespace SistemaAcademico.Models;

public class Inscripcion
{
    public int IdInscripcion { get; set; }
    public int IdMatricula { get; set; }
    public int IdAsignatura { get; set; }
    
    // Propiedades de navegación
    public string? NombreEstudiante { get; set; }
    public string? NombreAsignatura { get; set; }
}

public class CreateInscripcionDto
{
    public int IdMatricula { get; set; }
    public int IdAsignatura { get; set; }
}
