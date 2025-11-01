namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Representa a un cliente que será evaluado. Cada cliente está asociado a un
/// país y puede tener múltiples evaluaciones. Incluye datos de contacto y
/// campos de auditoría para identificar quién creó o modificó el registro.
/// </summary>
public class Cliente
{
    public int ClienteId { get; set; }
    public int PaisId { get; set; }
    public Pais Pais { get; set; } = null!;

    public string Nombre { get; set; } = null!;
    public string? Nit { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public bool Activo { get; set; } = true;

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    public ICollection<OeaEvaluacion> Evaluaciones { get; set; } = new List<OeaEvaluacion>();
}