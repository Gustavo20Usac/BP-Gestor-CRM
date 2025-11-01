namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Requisito individual que pertenece a una subsección. Puede incluir un
/// código identificador y una descripción extensa. Tiene trazabilidad y un
/// estado activo para desactivarlo sin eliminarlo.
/// </summary>
public class OeaRequisito
{
    public int RequisitoId { get; set; }
    public int SubseccionId { get; set; }
    public OeaSubseccion Subseccion { get; set; } = null!;

    public string? Codigo { get; set; }
    public string Descripcion { get; set; } = null!;
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    public ICollection<OeaEvaluacionDetalle> EvaluacionDetalles { get; set; } = new List<OeaEvaluacionDetalle>();
}