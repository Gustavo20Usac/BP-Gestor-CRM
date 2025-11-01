namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Representa un país en el catálogo. Permite asociar requisitos y clientes
/// a un ámbito geográfico determinado. Incluye información de auditoría para
/// saber quién creó o modificó la entidad y cuándo se hizo.
/// </summary>
public class Pais
{
    public int PaisId { get; set; }
    public string CodigoIso2 { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public bool Activo { get; set; } = true;

    // Auditoría
    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    // Relaciones de navegación
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public ICollection<OeaVersion> Versiones { get; set; } = new List<OeaVersion>();
}