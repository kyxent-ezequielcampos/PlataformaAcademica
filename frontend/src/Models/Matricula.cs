using System;

namespace frontend.Models;

public class Matricula
{
    public int IdMatricula { get; set; }
    public int IdEstudiante { get; set; }
    public int IdGrado { get; set; }
    public string CicloEscolar { get; set; } = string.Empty;
    public DateTime FechaMatricula { get; set; }
    public string Estado { get; set; } = "activo";
    public string? NombreEstudiante { get; set; }
    public string? NombreGrado { get; set; }
}
