namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class DocenteController
{
    private readonly DocenteRepository _repository;

    public DocenteController(DocenteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Docente>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Docente?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<int> Create(CreateDocenteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Documento) || 
            string.IsNullOrWhiteSpace(dto.Nombres) || 
            string.IsNullOrWhiteSpace(dto.Apellidos))
        {
            throw new ArgumentException("Faltan campos obligatorios");
        }

        return await _repository.Create(dto);
    }

    public async Task<bool> Update(int id, CreateDocenteDto dto)
    {
        var existe = await _repository.GetById(id);
        if (existe == null)
        {
            throw new KeyNotFoundException("Docente no encontrado");
        }

        return await _repository.Update(id, dto);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}