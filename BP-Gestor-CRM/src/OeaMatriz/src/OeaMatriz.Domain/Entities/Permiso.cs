namespace OeaMatriz.Domain.Entities;

/// <summary>
/// Representa un permiso de sistema. Los permisos definen acciones específicas
/// que los perfiles pueden realizar (ej. administrar usuarios, ver evaluaciones,
/// crear evaluaciones). Se asocian a los perfiles mediante la entidad
/// <see cref="PerfilPermiso"/>.
/// </summary>
public class Permiso
{
    public int PermisoId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }

    public ICollection<PerfilPermiso> PerfilPermisos { get; set; } = new List<PerfilPermiso>();
}