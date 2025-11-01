using OeaMatriz.Application.Common.Interfaces;

namespace OeaMatriz.Application.Evaluaciones.Services;

/// <summary>
/// Servicio que calcula el avance de una evaluación por sección. Se utiliza
/// para generar reportes de porcentaje de cumplimiento por cada grupo de
/// requisitos.
/// </summary>
public class EvaluacionAvanceService
{
    private readonly IEvaluacionDetalleReader _reader;

    public EvaluacionAvanceService(IEvaluacionDetalleReader reader)
    {
        _reader = reader;
    }

    /// <summary>
    /// Devuelve una colección con el avance por sección de la evaluación.
    /// </summary>
    public async Task<IReadOnlyList<AvancePorSeccionDto>> CalcularPorSeccionAsync(int evaluacionId, CancellationToken ct = default)
    {
        var detalles = await _reader.GetDetallesAsync(evaluacionId, ct);

        var grupos = detalles
            .GroupBy(d => new { d.SeccionId, d.SeccionNombre })
            .Select(g => new AvancePorSeccionDto
            {
                SeccionId = g.Key.SeccionId,
                SeccionNombre = g.Key.SeccionNombre,
                Total = g.Count(),
                Cumple = g.Count(x => x.Estado == 1),
                Parcial = g.Count(x => x.Estado == 2),
                NoCumple = g.Count(x => x.Estado == 3)
            })
            .ToList();

        foreach (var grupo in grupos)
        {
            grupo.PorcentajeCumple = grupo.Total == 0 ? 0 : Math.Round((double)grupo.Cumple / grupo.Total * 100, 2);
        }

        return grupos;
    }
}

/// <summary>
/// DTO utilizado para exponer el avance por sección de una evaluación.
/// </summary>
public class AvancePorSeccionDto
{
    public int SeccionId { get; set; }
    public string SeccionNombre { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Cumple { get; set; }
    public int Parcial { get; set; }
    public int NoCumple { get; set; }
    public double PorcentajeCumple { get; set; }
}