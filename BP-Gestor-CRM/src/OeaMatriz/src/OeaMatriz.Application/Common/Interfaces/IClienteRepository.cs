using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Application.Common.Interfaces;

/// <summary>
/// Contrato para operaciones CRUD sobre clientes. Proporciona métodos para
/// listar, obtener, crear, actualizar y eliminar clientes. Permite además
/// operaciones filtradas por usuario/cliente para cumplir con las reglas
/// de negocio sobre acceso restringido.
/// </summary>
public interface IClienteRepository
{
    Task<List<Cliente>> GetAllAsync(CancellationToken ct = default);
    Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Cliente cliente, CancellationToken ct = default);
    Task UpdateAsync(Cliente cliente, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}