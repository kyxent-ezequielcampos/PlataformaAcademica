namespace SistemaAcademico.Models;

public class Grado
{
    public int IdGrado { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Nivel { get; set; }
    public string? Seccion { get; set; }
    public bool Activo { get; set; }
}

public class CreateGradoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Nivel { get; set; }
    public string? Seccion { get; set; }
}
