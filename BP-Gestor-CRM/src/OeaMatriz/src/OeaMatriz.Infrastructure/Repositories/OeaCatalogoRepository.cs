using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.Infrastructure.Repositories;

/// <summary>
/// Implementación concreta del repositorio de catálogo OEA. Utiliza
/// Entity Framework Core para obtener secciones, subsecciones y requisitos
/// activos de una versión específica del catálogo.
/// </summary>
public class OeaCatalogoRepository : IOeaCatalogoRepository
{
    private readonly OeaDbContext _context;

    public OeaCatalogoRepository(OeaDbContext context)
    {
        _context = context;
    }

    public async Task<List<OeaSeccion>> GetSeccionesByVersionAsync(int versionId, CancellationToken ct = default)
    {
        return await _context.OeaSecciones
            .Include(s => s.Subsecciones.Where(x => x.Activo))
            .Where(s => s.VersionId == versionId && s.Activo)
            .OrderBy(s => s.Orden)
            .ToListAsync(ct);
    }

    public async Task<List<OeaSubseccion>> GetSubseccionesBySeccionAsync(int seccionId, CancellationToken ct = default)
    {
        return await _context.OeaSubsecciones
            .Include(s => s.Requisitos.Where(r => r.Activo))
            .Where(s => s.SeccionId == seccionId && s.Activo)
            .OrderBy(s => s.Orden)
            .ToListAsync(ct);
    }

    public async Task<List<OeaRequisito>> GetRequisitosBySubseccionAsync(int subseccionId, CancellationToken ct = default)
    {
        return await _context.OeaRequisitos
            .Where(r => r.SubseccionId == subseccionId && r.Activo)
            .OrderBy(r => r.Orden)
            .ToListAsync(ct);
    }
}