namespace SistemaAcademico.Models;

public class Asignatura
{
    public int IdAsignatura { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Creditos { get; set; }
    public bool Activo { get; set; }
}

public class CreateAsignaturaDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Creditos { get; set; } = 1;
}