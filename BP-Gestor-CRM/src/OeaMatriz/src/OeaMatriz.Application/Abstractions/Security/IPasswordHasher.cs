// OeaMatriz.Application/Abstractions/Security/IPasswordHasher.cs
namespace OeaMatriz.Application.Abstractions.Security;
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
