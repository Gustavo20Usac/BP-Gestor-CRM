namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Representa una versión del catálogo OEA para un país específico. Permite
/// gestionar cambios a lo largo del tiempo y vincular cada versión con sus
/// secciones y evaluaciones. Posee trazabilidad de creación y modificación.
/// </summary>
public class OeaVersion
{
    public int VersionId { get; set; }
    public string Nombre { get; set; } = null!;
    public int PaisId { get; set; }
    public Pais Pais { get; set; } = null!;
    public DateTime VigenteDesde { get; set; }
    public bool EsVigente { get; set; }

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    public ICollection<OeaSeccion> Secciones { get; set; } = new List<OeaSeccion>();
    public ICollection<OeaEvaluacion> Evaluaciones { get; set; } = new List<OeaEvaluacion>();
}