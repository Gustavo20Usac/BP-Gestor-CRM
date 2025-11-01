namespace OeaMatriz.Domain.Entities;

/// <summary>
/// User del sistema. Se relaciona con un perfil que define los permisos y
/// opcionalmente con un país para delimitar su contexto predeterminado. Contiene
/// campos de auditoría para registro de creación y modificación.
/// </summary>
public class Usuario
{
    public int UsuarioId { get; set; }
/// <summary>
/// Nombre de usuario utilizado para iniciar sesión. Corresponde a la columna
/// "User" en la base de datos.
/// </summary>
public string User { get; set; } = null!;
    public string ClaveHash { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string? Correo { get; set; }

    public int PerfilId { get; set; }
    public Perfil Perfil { get; set; } = null!;

    public int? PaisId { get; set; }
    public Pais? Pais { get; set; }

    /// <summary>
    /// Identificador del cliente asociado a este usuario cuando el perfil es de tipo cliente. Si el usuario es
    /// administrador o evaluador global, este valor puede ser nulo. Se utiliza para restringir el acceso
    /// a evaluaciones y datos del cliente.
    /// </summary>
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public bool Activo { get; set; } = true;

    public int? CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; }
    public int? ModificadoPor { get; set; }
    public DateTime? ModificadoEn { get; set; }
}