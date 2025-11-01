namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Representa una evaluación completa realizada a un cliente para una versión
/// específica del catálogo. Contiene observaciones generales y el estado de
/// proceso. También mantiene trazabilidad de creación y modificación.
/// </summary>
public class OeaEvaluacion
{
    public int EvaluacionId { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int VersionId { get; set; }
    public OeaVersion Version { get; set; } = null!;

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public DateTime FechaEval { get; set; }
    public string? ObservacionGral { get; set; }
    public string Estado { get; set; } = "CERRADA";

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    public ICollection<OeaEvaluacionDetalle> Detalles { get; set; } = new List<OeaEvaluacionDetalle>();
}