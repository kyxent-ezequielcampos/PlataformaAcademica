namespace SistemaAcademico.Models;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class CreateUsuarioDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}

public class LoginDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
}