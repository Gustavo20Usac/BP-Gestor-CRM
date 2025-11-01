using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OeaMatriz.Application.Common.Interfaces;
using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Infrastructure.Services;

/// <summary>
/// Concrete implementation of <see cref="ITokenService"/> that issues JWT tokens
/// using settings from the application configuration. The resulting token
/// contains the user's identifier, username and role (perfil).
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration used to retrieve JWT settings.</param>
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public string GenerateToken(Usuario user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        // Prepare the claims to embed in the token. Include the user ID, username and
        // associated perfil/role. Additional claims can be added as needed.
        var claims = new List<Claim>
        {
            // Unique identifier for the user
            new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
            // Username claim. Use the "User" property since the domain model defines it
            new Claim(ClaimTypes.Name, user.User),
        };

        // Include the profile/role name as role claim
        if (user.Perfil != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Perfil.Nombre));
            // Include the profile ID as custom claim for authorization logic if needed
            claims.Add(new Claim("PerfilId", user.PerfilId.ToString()));
        }

        // Include ClienteId if the user is associated with a client
        if (user.ClienteId.HasValue)
        {
            claims.Add(new Claim("ClienteId", user.ClienteId.Value.ToString()));
        }

        // Obtain signing key and issuer information from configuration. Keys should be
        // sufficiently long and secret. In production scenarios the key should be
        // stored securely, e.g. in Azure Key Vault or environment variables.
        var keyString = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer))
        {
            throw new InvalidOperationException("JWT configuration (Jwt:Key and Jwt:Issuer) must be provided");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}