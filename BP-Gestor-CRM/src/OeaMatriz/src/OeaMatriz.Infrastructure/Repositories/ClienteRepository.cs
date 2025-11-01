using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de clientes utilizando Entity Framework Core.
/// Gestiona operaciones CRUD sobre la entidad <see cref="Cliente"/>.
/// </summary>
public class ClienteRepository : IClienteRepository
{
    private readonly OeaDbContext _context;

    public ClienteRepository(OeaDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Cliente>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Clientes.AsNoTracking().ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.ClienteId == id, ct);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(Cliente cliente, CancellationToken ct = default)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(ct);
        return cliente.ClienteId;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Cliente cliente, CancellationToken ct = default)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Clientes.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.Clientes.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}