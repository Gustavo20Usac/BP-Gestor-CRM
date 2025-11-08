
namespace OeaMatriz.Domain.Entities.DTOs
{
    
        public class LoginRequest
        {
            public string CorreoElectronico { get; set; } = string.Empty;
            public string Contrasena { get; set; } = string.Empty;
            public int IdTenant { get; set; }
            public string? Idioma { get; set; } = string.Empty;
        }
    
}
