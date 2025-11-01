using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using System.Security.Claims;

namespace OeaMatriz.WebApi.Controllers;

/// <summary>
/// Controlador para gestionar usuarios. Incluye operaciones CRUD básicas y
/// aplica reglas de negocio para asociar usuarios de tipo cliente a un
/// cliente específico. El acceso está restringido a administradores salvo
/// para obtener la propia información del usuario.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repository;
    private readonly IClienteRepository _clienteRepo;
    private readonly IPerfilRepository _perfilRepo;

    public UsuariosController(IUsuarioRepository repository, IClienteRepository clienteRepo, IPerfilRepository perfilRepo)
    {
        _repository = repository;
        _clienteRepo = clienteRepo;
        _perfilRepo = perfilRepo;
    }

    /// <summary>
    /// Devuelve todos los usuarios. Solo los administradores pueden ver
    /// la lista completa.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (!string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }
        var usuarios = await _repository.GetAllAsync();
        return Ok(usuarios);
    }

    /// <summary>
    /// Obtiene un usuario por id. Los administradores pueden obtener cualquier usuario.
    /// Un usuario puede obtener su propia información.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (!int.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }
        if (!string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase) && currentUserId != id)
        {
            return Forbid();
        }
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null) return NotFound();
        return Ok(usuario);
    }

    /// <summary>
    /// Crea un nuevo usuario. Solo administradores pueden crear usuarios. Si el
    /// perfil es de tipo cliente, se requiere especificar el ClienteId.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Post([FromBody] Usuario usuario)
    {
        // Validación básica: perfil existe
        var perfil = await _perfilRepo.GetByIdAsync(usuario.PerfilId);
        if (perfil == null) return BadRequest("Perfil no válido.");
        // Si perfil es cliente (id 3), debe tener ClienteId
        if (string.Equals(perfil.Nombre, "Cliente", StringComparison.OrdinalIgnoreCase))
        {
            if (!usuario.ClienteId.HasValue)
            {
                return BadRequest("Los usuarios de perfil Cliente deben tener asignado un ClienteId.");
            }
            // Verificar que el cliente existe
            var cliente = await _clienteRepo.GetByIdAsync(usuario.ClienteId.Value);
            if (cliente == null)
            {
                return BadRequest("Cliente no válido para este usuario.");
            }
        }
        var id = await _repository.CreateAsync(usuario);
        return CreatedAtAction(nameof(GetById), new { id }, usuario);
    }

    /// <summary>
    /// Actualiza un usuario. Solo administradores pueden actualizar.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
    {
        if (id != usuario.UsuarioId)
        {
            return BadRequest("ID mismatch");
        }
        // Validación similar a creación
        var perfil = await _perfilRepo.GetByIdAsync(usuario.PerfilId);
        if (perfil == null) return BadRequest("Perfil no válido.");
        if (string.Equals(perfil.Nombre, "Cliente", StringComparison.OrdinalIgnoreCase))
        {
            if (!usuario.ClienteId.HasValue)
            {
                return BadRequest("Los usuarios de perfil Cliente deben tener asignado un ClienteId.");
            }
            var cliente = await _clienteRepo.GetByIdAsync(usuario.ClienteId.Value);
            if (cliente == null)
            {
                return BadRequest("Cliente no válido para este usuario.");
            }
        }
        await _repository.UpdateAsync(usuario);
        return NoContent();
    }

    /// <summary>
    /// Elimina un usuario existente. Solo administradores pueden eliminar usuarios.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}