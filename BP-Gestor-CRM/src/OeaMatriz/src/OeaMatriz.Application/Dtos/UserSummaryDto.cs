// OeaMatriz.Application/Users/Dtos/UserSummaryDto.cs
namespace OeaMatriz.Application.Users.Dtos;
public sealed record UserSummaryDto(int UsuarioId, string User, string NombreCompleto, int PerfilId, int? ClienteId, int PaisId, bool Activo);
