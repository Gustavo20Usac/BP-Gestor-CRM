using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;

namespace OeaMatriz.WebApi.Controllers;

/// <summary>
/// Controlador para gestión de permisos. Permite listar, obtener, crear,
/// actualizar y eliminar permisos. Todas las operaciones están restringidas
/// a usuarios con rol de administrador.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class PermisosController : ControllerBase
{
    private readonly IPermisoRepository _permisoRepo;

    public PermisosController(IPermisoRepository permisoRepo)
    {
        _permisoRepo = permisoRepo;
    }

    /// <summary>
    /// Lista todos los permisos existentes.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var permisos = await _permisoRepo.GetAllAsync();
        return Ok(permisos);
    }

    /// <summary>
    /// Obtiene un permiso por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var permiso = await _permisoRepo.GetByIdAsync(id);
        if (permiso == null) return NotFound();
        return Ok(permiso);
    }

    /// <summary>
    /// Crea un nuevo permiso.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Permiso permiso)
    {
        var id = await _permisoRepo.CreateAsync(permiso);
        return CreatedAtAction(nameof(GetById), new { id }, permiso);
    }

    /// <summary>
    /// Actualiza un permiso.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Permiso permiso)
    {
        if (id != permiso.PermisoId) return BadRequest("ID mismatch");
        await _permisoRepo.UpdateAsync(permiso);
        return NoContent();
    }

    /// <summary>
    /// Elimina un permiso.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _permisoRepo.DeleteAsync(id);
        return NoContent();
    }
}