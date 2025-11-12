namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class MatriculaController
{
    private readonly MatriculaRepository _repository;

    public MatriculaController(MatriculaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Matricula>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Matricula?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<int> Create(CreateMatriculaDto dto)
    {
        if (dto.IdEstudiante <= 0 || dto.IdGrado <= 0)
        {
            throw new ArgumentException("Estudiante y grado son obligatorios");
        }

        if (string.IsNullOrWhiteSpace(dto.CicloEscolar))
        {
            throw new ArgumentException("El ciclo escolar es obligatorio");
        }

        return await _repository.Create(dto);
    }

    public async Task<bool> Update(int id, CreateMatriculaDto dto)
    {
        var existe = await _repository.GetById(id);
        if (existe == null)
        {
            throw new KeyNotFoundException("Matrícula no encontrada");
        }

        return await _repository.Update(id, dto);
    }

    public async Task<bool> UpdateEstado(int id, string estado)
    {
        var existe = await _repository.GetById(id);
        if (existe == null)
        {
            throw new KeyNotFoundException("Matrícula no encontrada");
        }

        return await _repository.UpdateEstado(id, estado);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
