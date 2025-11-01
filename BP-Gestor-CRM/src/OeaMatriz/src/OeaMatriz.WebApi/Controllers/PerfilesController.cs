using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.WebApi.Controllers;

/// <summary>
/// Controlador para gestionar perfiles. Operaciones restringidas a usuarios
/// administradores. Permite crear, actualizar, obtener y eliminar perfiles,
/// incluyendo la asignación de permisos a cada perfil.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class PerfilesController : ControllerBase
{
    private readonly IPerfilRepository _perfilRepo;
    private readonly IPermisoRepository _permisoRepo;
    private readonly OeaDbContext _context;

    public PerfilesController(IPerfilRepository perfilRepo, IPermisoRepository permisoRepo, OeaDbContext context)
    {
        _perfilRepo = perfilRepo;
        _permisoRepo = permisoRepo;
        _context = context;
    }

    /// <summary>
    /// Devuelve la lista de perfiles junto con sus permisos asociados.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var perfiles = await _perfilRepo.GetAllAsync();
        return Ok(perfiles);
    }

    /// <summary>
    /// Obtiene un perfil por id incluyendo sus permisos.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var perfil = await _perfilRepo.GetByIdAsync(id);
        if (perfil == null) return NotFound();
        return Ok(perfil);
    }

    /// <summary>
    /// Crea un nuevo perfil con sus permisos asociados. Recibe un DTO con el
    /// nombre, descripción, rol admin y lista de IDs de permisos a asociar.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PerfilCreateDto dto)
    {
        // Crear entidad perfil
        var perfil = new Perfil
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            EsAdmin = dto.EsAdmin,
            Activo = dto.Activo,
            CreadoEn = DateTime.UtcNow
        };
        // Persistir perfil primero para obtener su ID
        var perfilId = await _perfilRepo.CreateAsync(perfil);
        // Asignar permisos
        foreach (var permId in dto.PermisoIds.Distinct())
        {
            var permiso = await _permisoRepo.GetByIdAsync(permId);
            if (permiso != null)
            {
                _context.PerfilPermisos.Add(new PerfilPermiso { PerfilId = perfilId, PermisoId = permId });
            }
        }
        await _context.SaveChangesAsync();
        var created = await _perfilRepo.GetByIdAsync(perfilId);
        return CreatedAtAction(nameof(GetById), new { id = perfilId }, created);
    }

    /// <summary>
    /// Actualiza un perfil existente y sus permisos. Reemplaza las asignaciones de permisos
    /// actuales por las nuevas proporcionadas en el DTO.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] PerfilCreateDto dto)
    {
        var perfil = await _perfilRepo.GetByIdAsync(id);
        if (perfil == null) return NotFound();
        perfil.Nombre = dto.Nombre;
        perfil.Descripcion = dto.Descripcion;
        perfil.EsAdmin = dto.EsAdmin;
        perfil.Activo = dto.Activo;
        // Actualizar entidad
        await _perfilRepo.UpdateAsync(perfil);
        // Limpiar permisos existentes
        var existentes = _context.PerfilPermisos.Where(pp => pp.PerfilId == id).ToList();
        _context.PerfilPermisos.RemoveRange(existentes);
        // Asignar nuevos permisos
        foreach (var permId in dto.PermisoIds.Distinct())
        {
            var permiso = await _permisoRepo.GetByIdAsync(permId);
            if (permiso != null)
            {
                _context.PerfilPermisos.Add(new PerfilPermiso { PerfilId = id, PermisoId = permId });
            }
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Elimina un perfil existente. Elimina también las relaciones con permisos.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var perfil = await _perfilRepo.GetByIdAsync(id);
        if (perfil == null) return NotFound();
        // Eliminar relaciones
        var existentes = _context.PerfilPermisos.Where(pp => pp.PerfilId == id).ToList();
        _context.PerfilPermisos.RemoveRange(existentes);
        await _context.SaveChangesAsync();
        // Eliminar perfil
        await _perfilRepo.DeleteAsync(id);
        return NoContent();
    }
}

/// <summary>
/// DTO para crear o actualizar perfiles. Incluye el nombre, descripción,
/// indicador de administrador, estado activo y una lista de IDs de permisos
/// que se deben asociar al perfil.
/// </summary>
public class PerfilCreateDto
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool EsAdmin { get; set; } = false;
    public bool Activo { get; set; } = true;
    public List<int> PermisoIds { get; set; } = new();
}