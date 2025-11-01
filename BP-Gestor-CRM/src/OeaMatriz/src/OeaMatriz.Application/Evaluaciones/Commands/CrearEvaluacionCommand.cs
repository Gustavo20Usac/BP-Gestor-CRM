namespace OeaMatriz.Application.Evaluaciones.Commands;

/// <summary>
/// DTO para crear una evaluación. Contiene la información del cliente,
/// versión y usuario, así como una colección de detalles por requisito.
/// </summary>
public sealed class CrearEvaluacionCommand
{
    public int ClienteId { get; set; }
    public int VersionId { get; set; }
    public int UsuarioId { get; set; }
    public string? ObservacionGral { get; set; }
    public Dictionary<int, EvaluacionDetalleDto> Detalles { get; set; } = new();
}

/// <summary>
/// DTO para representar el detalle de un requisito en una evaluación.
/// </summary>
public sealed class EvaluacionDetalleDto
{
    public byte Estado { get; set; }
    public string? Observaciones { get; set; }
}