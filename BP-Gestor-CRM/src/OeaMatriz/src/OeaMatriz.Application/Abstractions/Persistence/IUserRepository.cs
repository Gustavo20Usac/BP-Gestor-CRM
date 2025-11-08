// OeaMatriz.Application/Abstractions/Persistence/IUserRepository.cs
using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Application.Abstractions.Persistence;
public interface IUserRepository
{
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> GetByUserAsync(string user, CancellationToken ct = default);
    Task<bool> ExistsByUserAsync(string user, CancellationToken ct = default);
    Task<List<string>> GetUserPermissionsAsync(int usuarioId, CancellationToken ct = default);
    Task AddAsync(Usuario user, CancellationToken ct = default);
    Task UpdateAsync(Usuario user, CancellationToken ct = default);
}
