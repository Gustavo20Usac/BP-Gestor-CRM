namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Subsección dentro de una sección del catálogo OEA. Agrupa requisitos de
/// detalle y mantiene un orden dentro de la sección. Contiene trazabilidad.
/// </summary>
public class OeaSubseccion
{
    public int SubseccionId { get; set; }
    public int SeccionId { get; set; }
    public OeaSeccion Seccion { get; set; } = null!;

    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    public ICollection<OeaRequisito> Requisitos { get; set; } = new List<OeaRequisito>();
}