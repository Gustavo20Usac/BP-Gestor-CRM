namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Detalle de una evaluación asociada a un requisito. Guarda el estado de
/// cumplimiento, observaciones y evidencia opcional, además de campos de
/// auditoría para rastrear la creación y modificación.
/// </summary>
public class OeaEvaluacionDetalle
{
    public int EvaluacionDetId { get; set; }
    public int EvaluacionId { get; set; }
    public OeaEvaluacion Evaluacion { get; set; } = null!;

    public int RequisitoId { get; set; }
    public OeaRequisito Requisito { get; set; } = null!;

    public byte Estado { get; set; }
    public string? Observaciones { get; set; }
    public string? EvidenciaUrl { get; set; }

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }
}