using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.Infrastructure.Repositories;

/// <summary>
/// Implementación concreta de IEvaluacionRepository. Encapsula las
/// operaciones de lectura y escritura para evaluaciones y sus detalles
/// mediante Entity Framework Core.
/// </summary>
public class EvaluacionRepository : IEvaluacionRepository
{
    private readonly OeaDbContext _context;

    public EvaluacionRepository(OeaDbContext context)
    {
        _context = context;
    }

    public async Task<OeaEvaluacion?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.OeaEvaluaciones
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Requisito)
            .FirstOrDefaultAsync(e => e.EvaluacionId == id, ct);
    }

    public async Task<int> CreateAsync(OeaEvaluacion evaluacion, CancellationToken ct = default)
    {
        _context.OeaEvaluaciones.Add(evaluacion);
        await _context.SaveChangesAsync(ct);
        return evaluacion.EvaluacionId;
    }

    public async Task UpdateAsync(OeaEvaluacion evaluacion, CancellationToken ct = default)
    {
        _context.OeaEvaluaciones.Update(evaluacion);
        await _context.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<List<OeaEvaluacion>> GetByClienteAsync(int clienteId, CancellationToken ct = default)
    {
        return await _context.OeaEvaluaciones
            .Where(e => e.ClienteId == clienteId)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Requisito)
            .Include(e => e.Usuario)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}