using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de permisos usando Entity Framework Core.
/// Permite consultar y mantener la tabla de permisos.
/// </summary>
public class PermisoRepository : IPermisoRepository
{
    private readonly OeaDbContext _context;

    public PermisoRepository(OeaDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Permiso>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Permisos.AsNoTracking().ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<Permiso?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Permisos.AsNoTracking().FirstOrDefaultAsync(p => p.PermisoId == id, ct);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(Permiso permiso, CancellationToken ct = default)
    {
        _context.Permisos.Add(permiso);
        await _context.SaveChangesAsync(ct);
        return permiso.PermisoId;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Permiso permiso, CancellationToken ct = default)
    {
        _context.Permisos.Update(permiso);
        await _context.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Permisos.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.Permisos.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}