namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Describe el perfil de usuario dentro del sistema. Los perfiles definen niveles de
/// acceso como Administrador, Evaluador o Consulta. Se implementa trazabilidad
/// mediante campos de auditoría.
/// </summary>
public class Perfil
{
    public int PerfilId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool EsAdmin { get; set; }
    public bool Activo { get; set; } = true;

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    /// <summary>
    /// Colección de permisos asignados a este perfil mediante la tabla puente
    /// <see cref="PerfilPermiso"/>. Un perfil puede tener múltiples permisos.
    /// </summary>
    public ICollection<PerfilPermiso> PerfilPermisos { get; set; } = new List<PerfilPermiso>();
}