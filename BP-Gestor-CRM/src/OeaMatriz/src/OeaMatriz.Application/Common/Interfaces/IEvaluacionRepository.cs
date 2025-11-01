using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Application.Common.Interfaces;

/// <summary>
/// Contrato para persistencia de evaluaciones. Permite obtener una
/// evaluación completa, crear nuevas y actualizar existentes.
/// </summary>
public interface IEvaluacionRepository
{
    Task<OeaEvaluacion?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(OeaEvaluacion evaluacion, CancellationToken ct = default);
    Task UpdateAsync(OeaEvaluacion evaluacion, CancellationToken ct = default);

    /// <summary>
    /// Obtiene todas las evaluaciones asociadas a un cliente específico. Se
    /// incluye la entidad Cliente y User para mayor contexto.
    /// </summary>
    Task<List<OeaEvaluacion>> GetByClienteAsync(int clienteId, CancellationToken ct = default);
}