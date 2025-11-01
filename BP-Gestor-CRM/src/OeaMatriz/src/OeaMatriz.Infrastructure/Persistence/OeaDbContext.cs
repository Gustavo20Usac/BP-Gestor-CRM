using Microsoft.EntityFrameworkCore;
using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Infrastructure.Persistence;

/// <summary>
/// Representa el contexto de base de datos para EF Core. Registra todas las
/// entidades y configura las restricciones como índices únicos y tipos de
/// columna. Incluye datos de seeding inicial como países y perfiles.
/// </summary>
public class OeaDbContext : DbContext
{
    public OeaDbContext(DbContextOptions<OeaDbContext> options) : base(options)
    {
    }

    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Perfil> Perfiles => Set<Perfil>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<OeaVersion> OeaVersiones => Set<OeaVersion>();
    public DbSet<OeaSeccion> OeaSecciones => Set<OeaSeccion>();
    public DbSet<OeaSubseccion> OeaSubsecciones => Set<OeaSubseccion>();
    public DbSet<OeaRequisito> OeaRequisitos => Set<OeaRequisito>();
    public DbSet<OeaEvaluacion> OeaEvaluaciones => Set<OeaEvaluacion>();
    public DbSet<OeaEvaluacionDetalle> OeaEvaluacionDetalles => Set<OeaEvaluacionDetalle>();

    // Permisos y tabla puente perfil-permiso
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<PerfilPermiso> PerfilPermisos => Set<PerfilPermiso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de tipos de columna para strings largas
        modelBuilder.Entity<OeaRequisito>()
            .Property(r => r.Descripcion)
            .HasColumnType("NVARCHAR(MAX)");

        modelBuilder.Entity<OeaEvaluacionDetalle>()
            .Property(d => d.Observaciones)
            .HasColumnType("NVARCHAR(MAX)");

        // Índice único para evitar duplicar requisitos en la misma evaluación
        modelBuilder.Entity<OeaEvaluacionDetalle>()
            .HasIndex(d => new { d.EvaluacionId, d.RequisitoId })
            .IsUnique();

        // Seed de datos mínimos para que la aplicación tenga contexto inicial
        modelBuilder.Entity<Pais>().HasData(
            new Pais { PaisId = 1, CodigoIso2 = "GT", Nombre = "Guatemala", Activo = true, CreadoEn = DateTime.UtcNow },
            new Pais { PaisId = 2, CodigoIso2 = "SV", Nombre = "El Salvador", Activo = true, CreadoEn = DateTime.UtcNow }
        );

        modelBuilder.Entity<Perfil>().HasData(
            // Perfil administrador con acceso total
            new Perfil { PerfilId = 1, Nombre = "Administrador", Descripcion = "Perfil con acceso total", EsAdmin = true, Activo = true, CreadoEn = DateTime.UtcNow },
            // Perfil evaluador: puede crear y editar evaluaciones pero no administrar usuarios o clientes
            new Perfil { PerfilId = 2, Nombre = "Evaluador", Descripcion = "Puede crear y editar evaluaciones", EsAdmin = false, Activo = true, CreadoEn = DateTime.UtcNow },
            // Perfil cliente: usuario vinculado a un cliente, solo puede ver sus evaluaciones
            new Perfil { PerfilId = 3, Nombre = "Cliente", Descripcion = "Usuario asociado a un cliente, con acceso únicamente a sus evaluaciones", EsAdmin = false, Activo = true, CreadoEn = DateTime.UtcNow }
        );

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                UsuarioId = 1,
                // Nombre de usuario; corresponde a la columna "User" en la base de datos
                User = "admin",
                ClaveHash = "admin123$CAMBIAR",
                NombreCompleto = "Administrador del sistema",
                Correo = "admin@oea.local",
                PerfilId = 1,
                PaisId = 1,
                ClienteId = null,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<OeaVersion>().HasData(
            new OeaVersion { VersionId = 1, Nombre = "OEA-GT-2025", PaisId = 1, VigenteDesde = DateTime.UtcNow.Date, EsVigente = true, CreadoEn = DateTime.UtcNow }
        );

        modelBuilder.Entity<OeaSeccion>().HasData(
            new OeaSeccion { SeccionId = 1, VersionId = 1, Nombre = "SECCIÓN 1 - ASOCIADOS DEL NEGOCIO", Orden = 1, Activo = true, CreadoEn = DateTime.UtcNow },
            new OeaSeccion { SeccionId = 2, VersionId = 1, Nombre = "SECCIÓN 2 - RECURSOS HUMANOS", Orden = 2, Activo = true, CreadoEn = DateTime.UtcNow },
            new OeaSeccion { SeccionId = 3, VersionId = 1, Nombre = "SECCIÓN 3 - SEGURIDAD GENERAL / OPERACIONES", Orden = 3, Activo = true, CreadoEn = DateTime.UtcNow },
            new OeaSeccion { SeccionId = 4, VersionId = 1, Nombre = "SECCIÓN 4 - SEGURIDAD DEL TRANSPORTE DE LA CARGA", Orden = 4, Activo = true, CreadoEn = DateTime.UtcNow },
            new OeaSeccion { SeccionId = 5, VersionId = 1, Nombre = "SECCIÓN 5 - SEGURIDAD DE LAS INSTALACIONES", Orden = 5, Activo = true, CreadoEn = DateTime.UtcNow },
            new OeaSeccion { SeccionId = 6, VersionId = 1, Nombre = "SECCIÓN 6 - SEGURIDAD INDUSTRIAL Y EMERGENCIAS", Orden = 6, Activo = true, CreadoEn = DateTime.UtcNow },
            new OeaSeccion { SeccionId = 7, VersionId = 1, Nombre = "SECCIÓN 7 - SEGURIDAD DE LA INFORMACIÓN Y CIBERSEGURIDAD", Orden = 7, Activo = true, CreadoEn = DateTime.UtcNow },
            new OeaSeccion { SeccionId = 8, VersionId = 1, Nombre = "SECCIÓN 8 - SEGURIDAD DE MARCHAMOS Y PRECINTOS", Orden = 8, Activo = true, CreadoEn = DateTime.UtcNow }
        );

        // Permisos del sistema. Estos definen operaciones permitidas a los perfiles. Los nombres son arbitrarios y pueden
        // ajustarse según las necesidades. Se asignan más abajo mediante la tabla puente PerfilPermiso.
        modelBuilder.Entity<Permiso>().HasData(
            new Permiso { PermisoId = 1, Nombre = "ManageClients", Descripcion = "Administrar clientes", Activo = true, CreadoEn = DateTime.UtcNow },
            new Permiso { PermisoId = 2, Nombre = "ManageUsers", Descripcion = "Administrar usuarios", Activo = true, CreadoEn = DateTime.UtcNow },
            new Permiso { PermisoId = 3, Nombre = "ManageProfiles", Descripcion = "Administrar perfiles", Activo = true, CreadoEn = DateTime.UtcNow },
            new Permiso { PermisoId = 4, Nombre = "ManagePermissions", Descripcion = "Administrar permisos", Activo = true, CreadoEn = DateTime.UtcNow },
            new Permiso { PermisoId = 5, Nombre = "ViewEvaluations", Descripcion = "Ver evaluaciones", Activo = true, CreadoEn = DateTime.UtcNow },
            new Permiso { PermisoId = 6, Nombre = "CreateEvaluations", Descripcion = "Crear evaluaciones", Activo = true, CreadoEn = DateTime.UtcNow }
        );

        // Relaciones perfil-permiso. Asociamos todos los permisos al perfil administrador. El perfil evaluador y cliente
        // tendrán únicamente permisos para crear y ver evaluaciones.
        modelBuilder.Entity<PerfilPermiso>().HasData(
            // Admin: todos los permisos
            new PerfilPermiso { PerfilId = 1, PermisoId = 1 },
            new PerfilPermiso { PerfilId = 1, PermisoId = 2 },
            new PerfilPermiso { PerfilId = 1, PermisoId = 3 },
            new PerfilPermiso { PerfilId = 1, PermisoId = 4 },
            new PerfilPermiso { PerfilId = 1, PermisoId = 5 },
            new PerfilPermiso { PerfilId = 1, PermisoId = 6 },
            // Evaluador: ver y crear evaluaciones
            new PerfilPermiso { PerfilId = 2, PermisoId = 5 },
            new PerfilPermiso { PerfilId = 2, PermisoId = 6 },
            // Cliente: ver y crear evaluaciones (asociadas a sí mismo)
            new PerfilPermiso { PerfilId = 3, PermisoId = 5 },
            new PerfilPermiso { PerfilId = 3, PermisoId = 6 }
        );

        // Configuración de la tabla puente PerfilPermiso con clave compuesta
        modelBuilder.Entity<PerfilPermiso>()
            .HasKey(pp => new { pp.PerfilId, pp.PermisoId });

        modelBuilder.Entity<PerfilPermiso>()
            .HasOne(pp => pp.Perfil)
            .WithMany(p => p.PerfilPermisos)
            .HasForeignKey(pp => pp.PerfilId);

        modelBuilder.Entity<PerfilPermiso>()
            .HasOne(pp => pp.Permiso)
            .WithMany(p => p.PerfilPermisos)
            .HasForeignKey(pp => pp.PermisoId);

        base.OnModelCreating(modelBuilder);
    }
}