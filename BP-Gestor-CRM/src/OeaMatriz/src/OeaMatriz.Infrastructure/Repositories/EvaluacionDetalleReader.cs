using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.Infrastructure.Repositories;

/// <summary>
/// Implementación de IEvaluacionDetalleReader. Devuelve una lista de
/// proyecciones de detalles de evaluación con la información mínima
/// requerida para el cálculo de avance.
/// </summary>
public class EvaluacionDetalleReader : IEvaluacionDetalleReader
{
    private readonly OeaDbContext _context;

    public EvaluacionDetalleReader(OeaDbContext context)
    {
        _context = context;
    }

    public async Task<List<EvaluacionDetalleProjection>> GetDetallesAsync(int evaluacionId, CancellationToken ct = default)
    {
        return await _context.OeaEvaluacionDetalles
            .Where(d => d.EvaluacionId == evaluacionId)
            .Select(d => new EvaluacionDetalleProjection(
                d.Requisito.Subseccion.Seccion.SeccionId,
                d.Requisito.Subseccion.Seccion.Nombre,
                d.Estado
            ))
            .ToListAsync(ct);
    }
}