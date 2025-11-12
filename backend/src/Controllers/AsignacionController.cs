namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class AsignacionController
{
    private readonly AsignacionRepository _repository;

    public AsignacionController(AsignacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Asignacion>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Asignacion?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<int> Create(CreateAsignacionDto dto)
    {
        if (dto.IdDocente <= 0 || dto.IdAsignatura <= 0 || dto.IdGrado <= 0)
        {
            throw new ArgumentException("Todos los IDs son obligatorios");
        }

        if (string.IsNullOrWhiteSpace(dto.CicloEscolar))
        {
            throw new ArgumentException("El ciclo escolar es obligatorio");
        }

        return await _repository.Create(dto);
    }

    public async Task<bool> Update(int id, CreateAsignacionDto dto)
    {
        var existe = await _repository.GetById(id);
        if (existe == null)
        {
            throw new KeyNotFoundException("Asignación no encontrada");
        }

        return await _repository.Update(id, dto);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
