using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EvaluacionesController : ControllerBase
{
    private readonly IEvaluacionRepository _repository;
    private readonly OeaDbContext _context;

    public EvaluacionesController(IEvaluacionRepository repository, OeaDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    /// <summary>
    /// Obtiene una evaluación completa con su detalle por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var evaluacion = await _repository.GetByIdAsync(id);
        if (evaluacion == null) return NotFound();
        // Restricción: si no es admin, verificar que el clienteId coincida con el claim
        var role = User.FindFirstValue(ClaimTypes.Role);
        var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
        if (!string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase))
        {
            if (clienteIdClaim == null || evaluacion.ClienteId.ToString() != clienteIdClaim)
            {
                return Forbid();
            }
        }
        return Ok(evaluacion);
    }

    /// <summary>
    /// Obtiene todas las evaluaciones asociadas a un cliente. Los administradores y evaluadores
    /// pueden consultar cualquier cliente. Los usuarios cliente solo su propio cliente.
    /// </summary>
    [HttpGet("cliente/{clienteId:int}")]
    public async Task<IActionResult> GetByCliente(int clienteId)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
        // Si no es admin ni evaluador, comprobar que el clienteId coincide
        bool isAdmin = string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase);
        bool isEvaluador = string.Equals(role, "Evaluador", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && !isEvaluador)
        {
            if (clienteIdClaim == null || clienteIdClaim != clienteId.ToString())
            {
                return Forbid();
            }
        }
        var evaluaciones = await _repository.GetByClienteAsync(clienteId);
        return Ok(evaluaciones);
    }
    /// <summary>
    /// Crea una nueva evaluación y sus detalles.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CrearEvaluacionDto dto)
    {
        // Validar existencia de referencias
        var version = await _context.OeaVersiones.FindAsync(dto.VersionId);
        var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
        var usuario = await _context.Usuarios.FindAsync(dto.UsuarioId);
        if (version == null || cliente == null || usuario == null)
        {
            return BadRequest("Datos de referencia no válidos.");
        }
        // Validar que el usuario que crea la evaluación tiene permisos: admin, evaluador o cliente del mismo cliente
        var currentRole = User.FindFirstValue(ClaimTypes.Role);
        var currentClienteId = User.FindFirst("ClienteId")?.Value;
        bool isAdmin = string.Equals(currentRole, "Administrador", StringComparison.OrdinalIgnoreCase);
        bool isEvaluador = string.Equals(currentRole, "Evaluador", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && !isEvaluador)
        {
            if (currentClienteId == null || currentClienteId != dto.ClienteId.ToString())
            {
                return Forbid();
            }
        }

        var evaluacion = new OeaEvaluacion
        {
            ClienteId = dto.ClienteId,
            VersionId = dto.VersionId,
            UsuarioId = dto.UsuarioId,
            FechaEval = DateTime.UtcNow,
            ObservacionGral = dto.ObservacionGral,
            Estado = "EN_PROCESO",
            CreadoPor = dto.UsuarioId,
            CreadoEn = DateTime.UtcNow
        };

        foreach (var detalle in dto.Detalles)
        {
            evaluacion.Detalles.Add(new OeaEvaluacionDetalle
            {
                RequisitoId = detalle.RequisitoId,
                Estado = detalle.Estado,
                Observaciones = detalle.Observaciones,
                CreadoPor = dto.UsuarioId,
                CreadoEn = DateTime.UtcNow
            });
        }

        var id = await _repository.CreateAsync(evaluacion);
        return CreatedAtAction(nameof(Get), new { id }, evaluacion);
    }
}

/// <summary>
/// DTO para crear una nueva evaluación a través de la API.
/// </summary>
public class CrearEvaluacionDto
{
    public int ClienteId { get; set; }
    public int VersionId { get; set; }
    public int UsuarioId { get; set; }
    public string? ObservacionGral { get; set; }
    public List<CrearEvaluacionDetalleDto> Detalles { get; set; } = new();
}

/// <summary>
/// DTO de detalle para la creación de evaluaciones.
/// </summary>
public class CrearEvaluacionDetalleDto
{
    public int RequisitoId { get; set; }
    public byte Estado { get; set; }
    public string? Observaciones { get; set; }
}