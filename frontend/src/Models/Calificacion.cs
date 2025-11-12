using System;

namespace frontend.Models;

public class Calificacion
{
    public int IdCalificacion { get; set; }
    public int IdInscripcion { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public decimal Nota { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? Observaciones { get; set; }
    public string? NombreEstudiante { get; set; }
    public string? NombreAsignatura { get; set; }
}
