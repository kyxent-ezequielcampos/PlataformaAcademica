using System;

namespace frontend.Models;

public class Asignacion
{
    public int IdAsignacion { get; set; }
    public int IdDocente { get; set; }
    public int IdAsignatura { get; set; }
    public int IdGrado { get; set; }
    public string CicloEscolar { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public string? NombreDocente { get; set; }
    public string? NombreAsignatura { get; set; }
    public string? NombreGrado { get; set; }
}
