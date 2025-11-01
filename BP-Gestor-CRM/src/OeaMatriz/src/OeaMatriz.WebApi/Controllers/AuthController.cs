using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;
using OeaMatriz.Infrastructure.Persistence;

namespace OeaMatriz.WebApi.Controllers;

/// <summary>
/// Provides authentication endpoints for the API. Handles login and returns
/// JWT tokens on successful authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly OeaDbContext _context;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="context">The database context used to query users.</param>
    /// <param name="tokenService">The token service used to generate JWT tokens.</param>
    public AuthController(OeaDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Authenticates a user by username and password. On success, returns a JWT token
    /// containing standard claims (sub, name, role). On failure, returns 401 Unauthorized.
    /// </summary>
    /// <param name="request">Login request containing username and password.</param>
    /// <returns>JWT token if authentication succeeds; 401 otherwise.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required.");
        }
        // Fetch the user including the perfil. Assumes passwords are stored as plain text
        // or hashed and stored in ClaveHash. In production, use a proper password hasher.
        // Buscar por el nombre de usuario en la propiedad "User" (antes UsuarioName)
        var user = await _context.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(u => u.User == request.Username);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }
        // TODO: replace this with proper hash verification
        if (!string.Equals(user.ClaveHash, request.Password, StringComparison.Ordinal))
        {
            return Unauthorized("Invalid username or password.");
        }
        var token = _tokenService.GenerateToken(user);
        return Ok(new LoginResponseDto { Token = token });
    }
}

/// <summary>
/// Represents the incoming login request payload.
/// </summary>
public sealed class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Represents the response containing a JWT token.
/// </summary>
public sealed class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
}