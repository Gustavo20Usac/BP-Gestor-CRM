// OeaMatriz.Application/Abstractions/Services/IAuthService.cs
namespace OeaMatriz.Application.Abstractions.Services;
public sealed record LoginRequest(string User, string Password);
public sealed record LoginResult(bool IsAuthenticated, int? UsuarioId, string? Nombre, int? PerfilId, int? PaisId, List<string>? Permisos, string? Error);

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
