using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de perfiles utilizando Entity Framework Core.
/// Proporciona operaciones CRUD sobre la entidad <see cref="Perfil"/>.
/// </summary>
public class PerfilRepository : IPerfilRepository
{
    private readonly OeaDbContext _context;

    public PerfilRepository(OeaDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Perfil>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Perfiles
            .Include(p => p.PerfilPermisos)
            .ThenInclude(pp => pp.Permiso)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<Perfil?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Perfiles
            .Include(p => p.PerfilPermisos)
            .ThenInclude(pp => pp.Permiso)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PerfilId == id, ct);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(Perfil perfil, CancellationToken ct = default)
    {
        _context.Perfiles.Add(perfil);
        await _context.SaveChangesAsync(ct);
        return perfil.PerfilId;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Perfil perfil, CancellationToken ct = default)
    {
        _context.Perfiles.Update(perfil);
        await _context.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Perfiles.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.Perfiles.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}