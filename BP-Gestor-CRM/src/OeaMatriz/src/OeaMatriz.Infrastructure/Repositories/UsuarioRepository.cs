using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de usuarios usando Entity Framework Core.
/// Proporciona operaciones CRUD básicas sobre la entidad <see cref="Usuario"/>.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly OeaDbContext _context;

    public UsuarioRepository(OeaDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Usuario>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Usuarios
            .Include(u => u.Perfil)
            .Include(u => u.Cliente)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Usuarios
            .Include(u => u.Perfil)
            .Include(u => u.Cliente)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UsuarioId == id, ct);
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(ct);
        return usuario.UsuarioId;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _context.Usuarios.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.Usuarios.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}