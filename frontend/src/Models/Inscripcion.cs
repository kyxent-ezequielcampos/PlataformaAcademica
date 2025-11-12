using System;

namespace frontend.Models;

public class Inscripcion
{
    public int IdInscripcion { get; set; }
    public int IdMatricula { get; set; }
    public int IdAsignatura { get; set; }
    public string? NombreEstudiante { get; set; }
    public string? NombreAsignatura { get; set; }
}
