namespace SistemaAcademico.Models;

public class Docente
{
    public int IdDocente { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Especialidad { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? FotoUrl { get; set; }
    public int? IdUsuario { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class CreateDocenteDto
{
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Especialidad { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? FotoUrl { get; set; }
    public int? IdUsuario { get; set; }
}
