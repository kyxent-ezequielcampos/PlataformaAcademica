namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class CalificacionController
{
    private readonly CalificacionRepository _repository;

    public CalificacionController(CalificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Calificacion>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Calificacion?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<List<Calificacion>> GetByInscripcion(int idInscripcion)
    {
        return await _repository.GetByInscripcion(idInscripcion);
    }

    public async Task<int> Create(CreateCalificacionDto dto)
    {
        if (dto.IdInscripcion <= 0)
        {
            throw new ArgumentException("La inscripción es obligatoria");
        }

        if (string.IsNullOrWhiteSpace(dto.Periodo))
        {
            throw new ArgumentException("El periodo es obligatorio");
        }

        if (dto.Nota < 0 || dto.Nota > 100)
        {
            throw new ArgumentException("La nota debe estar entre 0 y 100");
        }

        return await _repository.Create(dto);
    }

    public async Task<bool> Update(int id, CreateCalificacionDto dto)
    {
        var existe = await _repository.GetById(id);
        if (existe == null)
        {
            throw new KeyNotFoundException("Calificación no encontrada");
        }

        if (dto.Nota < 0 || dto.Nota > 100)
        {
            throw new ArgumentException("La nota debe estar entre 0 y 100");
        }

        return await _repository.Update(id, dto);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }

    public async Task<decimal> CalcularPromedioEstudiante(int idEstudiante, string cicloEscolar)
    {
        if (idEstudiante <= 0)
        {
            throw new ArgumentException("ID de estudiante inválido");
        }

        if (string.IsNullOrWhiteSpace(cicloEscolar))
        {
            throw new ArgumentException("El ciclo escolar es obligatorio");
        }

        return await _repository.CalcularPromedioEstudiante(idEstudiante, cicloEscolar);
    }
}
