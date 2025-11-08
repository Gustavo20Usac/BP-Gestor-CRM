// OeaMatriz.Infrastructure/Security/AuthService.cs
using OeaMatriz.Application.Abstractions.Persistence;
using OeaMatriz.Application.Abstractions.Security;
using OeaMatriz.Application.Abstractions.Services;

namespace OeaMatriz.Infrastructure.Security;
public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var u = await _users.GetByUserAsync(request.User, ct);
        if (u is null || !u.Activo)
            return new(false, null, null, null, null, null, "Usuario o contraseña inválidos.");

        if (!_hasher.Verify(request.Password, u.ClaveHash))
            return new(false, null, null, null, null, null, "Usuario o contraseña inválidos.");

        var permisos = await _users.GetUserPermissionsAsync(u.UsuarioId, ct);

        return new(true, u.UsuarioId, u.NombreCompleto, u.PerfilId, u.PaisId, permisos, null);
    }
}
