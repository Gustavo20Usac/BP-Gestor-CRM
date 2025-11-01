using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Application.Common.Interfaces;

/// <summary>
/// Contrato para operaciones CRUD sobre perfiles. Permite gestionar los
/// diferentes tipos de perfiles disponibles en el sistema, como
/// administrador, evaluador y cliente. También se podrían extender con
/// operaciones para asignar permisos a perfiles.
/// </summary>
public interface IPerfilRepository
{
    Task<List<Perfil>> GetAllAsync(CancellationToken ct = default);
    Task<Perfil?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Perfil perfil, CancellationToken ct = default);
    Task UpdateAsync(Perfil perfil, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}