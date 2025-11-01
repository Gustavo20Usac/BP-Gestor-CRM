namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Tabla puente que relaciona perfiles con permisos. Cada registro representa
/// la asignación de un permiso específico a un perfil determinado. Un perfil
/// puede tener múltiples permisos y cada permiso puede ser asignado a
/// múltiples perfiles. No contiene campos de auditoría ya que funciona
/// exclusivamente como relación N:N.
/// </summary>
public class PerfilPermiso
{
    /// <summary>
    /// Identificador del perfil asociado al permiso.
    /// </summary>
    public int PerfilId { get; set; }

    /// <summary>
    /// Identificador del permiso asignado al perfil.
    /// </summary>
    public int PermisoId { get; set; }

    /// <summary>
    /// Navegación al perfil correspondiente.
    /// </summary>
    public Perfil Perfil { get; set; } = null!;

    /// <summary>
    /// Navegación al permiso correspondiente.
    /// </summary>
    public Permiso Permiso { get; set; } = null!;
}