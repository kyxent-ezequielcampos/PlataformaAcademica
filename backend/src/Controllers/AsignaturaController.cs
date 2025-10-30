namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class AsignaturaController
{
    private readonly AsignaturaRepository _repository;

    public AsignaturaController(AsignaturaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Asignatura>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Asignatura?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<int> Create(CreateAsignaturaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Codigo) || 
            string.IsNullOrWhiteSpace(dto.Nombre))
        {
            throw new ArgumentException("Faltan campos obligatorios");
        }

        return await _repository.Create(dto);
    }

    public async Task<bool> Update(int id, CreateAsignaturaDto dto)
    {
        var existe = await _repository.GetById(id);
        if (existe == null)
        {
            throw new KeyNotFoundException("Asignatura no encontrada");
        }

        return await _repository.Update(id, dto);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}