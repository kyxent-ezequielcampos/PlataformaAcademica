namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class GradoController
{
    private readonly GradoRepository _repository;

    public GradoController(GradoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Grado>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Grado?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<int> Create(CreateGradoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            throw new ArgumentException("El nombre del grado es obligatorio");
        }

        return await _repository.Create(dto);
    }

    public async Task<bool> Update(int id, CreateGradoDto dto)
    {
        var existe = await _repository.GetById(id);
        if (existe == null)
        {
            throw new KeyNotFoundException("Grado no encontrado");
        }

        return await _repository.Update(id, dto);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
