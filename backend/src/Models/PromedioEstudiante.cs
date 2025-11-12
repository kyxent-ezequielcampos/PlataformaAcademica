namespace SistemaAcademico.Models;

public class PromedioEstudiante
{
    public int IdEstudiante { get; set; }
    public string CicloEscolar { get; set; } = string.Empty;
    public decimal Promedio { get; set; }
}

public class PromedioRequest
{
    public int IdEstudiante { get; set; }
    public string CicloEscolar { get; set; } = string.Empty;
}
