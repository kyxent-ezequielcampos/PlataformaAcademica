namespace SistemaAcademico.Controllers;

using SistemaAcademico.Models;
using SistemaAcademico.Repositories;

public class InscripcionController
{
    private readonly InscripcionRepository _repository;

    public InscripcionController(InscripcionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Inscripcion>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Inscripcion?> GetById(int id)
    {
        return await _repository.GetById(id);
    }

    public async Task<List<Inscripcion>> GetByMatricula(int idMatricula)
    {
        return await _repository.GetByMatricula(idMatricula);
    }

    public async Task<int> Create(CreateInscripcionDto dto)
    {
        if (dto.IdMatricula <= 0 || dto.IdAsignatura <= 0)
        {
            throw new ArgumentException("Matrícula y asignatura son obligatorias");
        }

        return await _repository.Create(dto);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
