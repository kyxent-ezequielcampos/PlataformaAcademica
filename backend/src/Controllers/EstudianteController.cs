namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class EstudianteController
{
    private readonly EstudianteRepository _repository;

    public EstudianteController(EstudianteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Estudiante>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Estudiante?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<int> Create(CreateEstudianteDto dto)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(dto.Documento) || 
            string.IsNullOrWhiteSpace(dto.Nombres) || 
            string.IsNullOrWhiteSpace(dto.Apellidos))
        {
            throw new ArgumentException("Faltan campos obligatorios");
        }

        return await _repository.Create(dto);
    }

    public async Task<bool> Update(int id, CreateEstudianteDto dto)
    {
        var existe = await _repository.GetById(id);
        if (existe == null)
        {
            throw new KeyNotFoundException("Estudiante no encontrado");
        }

        return await _repository.Update(id, dto);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}