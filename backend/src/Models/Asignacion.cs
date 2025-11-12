namespace SistemaAcademico.Models;

public class Asignacion
{
    public int IdAsignacion { get; set; }
    public int IdDocente { get; set; }
    public int IdAsignatura { get; set; }
    public int IdGrado { get; set; }
    public string CicloEscolar { get; set; } = string.Empty;
    public bool Activo { get; set; }
    
    // Propiedades de navegación para vistas
    public string? NombreDocente { get; set; }
    public string? NombreAsignatura { get; set; }
    public string? NombreGrado { get; set; }
}

public class CreateAsignacionDto
{
    public int IdDocente { get; set; }
    public int IdAsignatura { get; set; }
    public int IdGrado { get; set; }
    public string CicloEscolar { get; set; } = string.Empty;
}
