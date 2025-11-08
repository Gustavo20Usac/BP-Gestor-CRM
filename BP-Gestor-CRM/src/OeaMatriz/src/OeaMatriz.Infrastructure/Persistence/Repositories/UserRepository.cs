// OeaMatriz.Infrastructure/Persistence/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Abstractions.Persistence;
using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Infrastructure.Persistence.Repositories;
public class UserRepository : IUserRepository
{
    private readonly OeaDbContext _ctx;
    public UserRepository(OeaDbContext ctx) => _ctx = ctx;

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
        => _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.UsuarioId == id, ct);

    public Task<Usuario?> GetByUserAsync(string user, CancellationToken ct = default)
        => _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.User == user, ct);

    public Task<bool> ExistsByUserAsync(string user, CancellationToken ct = default)
        => _ctx.Usuarios.AsNoTracking().AnyAsync(x => x.User == user, ct);

    public async Task<List<string>> GetUserPermissionsAsync(int usuarioId, CancellationToken ct = default)
    {
        // Usuario -> Perfil -> PerfilPermiso -> Permiso
        var query =
            from u in _ctx.Usuarios.AsNoTracking()
            join p in _ctx.Perfiles.AsNoTracking() on u.PerfilId equals p.PerfilId
            join pp in _ctx.PerfilPermisos.AsNoTracking() on p.PerfilId equals pp.PerfilId
            join pe in _ctx.Permisos.AsNoTracking() on pp.PermisoId equals pe.PermisoId
            where u.UsuarioId == usuarioId && u.Activo && p.Activo && pe.Activo
            select pe.Nombre;

        return await query.Distinct().ToListAsync(ct);
    }

    public async Task AddAsync(Usuario user, CancellationToken ct = default)
    {
        await _ctx.Usuarios.AddAsync(user, ct);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Usuario user, CancellationToken ct = default)
    {
        _ctx.Usuarios.Update(user);
        await _ctx.SaveChangesAsync(ct);
    }
}
