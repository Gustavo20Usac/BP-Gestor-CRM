using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Application.Common.Interfaces;

/// <summary>
/// Contrato para operaciones de consulta y mantenimiento de permisos. Los
/// permisos representan acciones que pueden ser asignadas a perfiles.
/// </summary>
public interface IPermisoRepository
{
    Task<List<Permiso>> GetAllAsync(CancellationToken ct = default);
    Task<Permiso?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Permiso permiso, CancellationToken ct = default);
    Task UpdateAsync(Permiso permiso, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}