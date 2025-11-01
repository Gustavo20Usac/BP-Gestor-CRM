namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Sección del catálogo OEA asociada a una versión. Contiene múltiples
/// subsecciones. Se utiliza para agrupar los requisitos en categorías de alto
/// nivel. Incluye trazabilidad.
/// </summary>
public class OeaSeccion
{
    public int SeccionId { get; set; }
    public int VersionId { get; set; }
    public OeaVersion Version { get; set; } = null!;

    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    public ICollection<OeaSubseccion> Subsecciones { get; set; } = new List<OeaSubseccion>();
}