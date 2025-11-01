namespace OeaMatriz.Application.Common.Interfaces;

/// <summary>
/// Abstracción para leer los detalles de una evaluación, incluyendo la
/// información necesaria para calcular el avance. Devuelve proyecciones
/// simplificadas para no exponer entidades de infraestructura en la capa
/// de aplicación.
/// </summary>
public interface IEvaluacionDetalleReader
{
    Task<List<EvaluacionDetalleProjection>> GetDetallesAsync(int evaluacionId, CancellationToken ct = default);
}

/// <summary>
/// Proyección de detalle de evaluación utilizada para el cálculo de avance.
/// </summary>
public record EvaluacionDetalleProjection(int SeccionId, string SeccionNombre, byte Estado);