using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using System.Security.Claims;

namespace OeaMatriz.WebApi.Controllers;

/// <summary>
/// Controlador para gestión de clientes. Permite CRUD completo. Los usuarios con
/// perfil de Cliente solo podrán obtener su propio registro. Los administradores
/// podrán realizar todas las operaciones.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteRepository _repository;

    public ClientesController(IClienteRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Devuelve la lista de todos los clientes. Solo los administradores pueden
    /// ver la lista completa. Si el usuario tiene un cliente asociado,
    /// se devuelve únicamente ese cliente.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
        // Si es admin devolver todos
        if (string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase))
        {
            var clientes = await _repository.GetAllAsync();
            return Ok(clientes);
        }
        // Si tiene claim de cliente, devolver solo ese
        if (int.TryParse(clienteIdClaim, out var clienteId))
        {
            var cliente = await _repository.GetByIdAsync(clienteId);
            if (cliente == null) return NotFound();
            return Ok(new[] { cliente });
        }
        return Forbid();
    }

    /// <summary>
    /// Obtiene un cliente por su ID. Solo los administradores pueden obtener
    /// cualquier cliente; los usuarios cliente solo su propio registro.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
        // Admin
        if (string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase))
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }
        // Client user: only access own client
        if (int.TryParse(clienteIdClaim, out var clienteId) && clienteId == id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }
        return Forbid();
    }

    /// <summary>
    /// Crea un nuevo cliente. Solo disponible para administradores.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Post([FromBody] Cliente cliente)
    {
        var id = await _repository.CreateAsync(cliente);
        return CreatedAtAction(nameof(GetById), new { id }, cliente);
    }

    /// <summary>
    /// Actualiza un cliente existente. Solo disponible para administradores.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Put(int id, [FromBody] Cliente cliente)
    {
        if (id != cliente.ClienteId)
        {
            return BadRequest("ID mismatch");
        }
        await _repository.UpdateAsync(cliente);
        return NoContent();
    }

    /// <summary>
    /// Elimina un cliente existente. Solo disponible para administradores.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}