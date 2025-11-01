using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Application.Common.Interfaces;

/// <summary>
/// Contrato para operaciones CRUD sobre usuarios. Incluye métodos para
/// listar usuarios, obtener por id, crear, actualizar y eliminar. También
/// permite filtrar por cliente para restringir la visibilidad a usuarios
/// de un cliente específico.
/// </summary>
public interface IUsuarioRepository
{
    Task<List<Usuario>> GetAllAsync(CancellationToken ct = default);
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Usuario usuario, CancellationToken ct = default);
    Task UpdateAsync(Usuario usuario, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}