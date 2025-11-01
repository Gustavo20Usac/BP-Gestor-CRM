using OeaMatriz.Domain.Entities;

namespace OeaMatriz.Application.Common.Interfaces;

/// <summary>
/// Contrato para acceso al catálogo OEA. Permite consultar secciones,
/// subsecciones y requisitos para una versión particular del catálogo.
/// </summary>
public interface IOeaCatalogoRepository
{
    Task<List<OeaSeccion>> GetSeccionesByVersionAsync(int versionId, CancellationToken ct = default);
    Task<List<OeaSubseccion>> GetSubseccionesBySeccionAsync(int seccionId, CancellationToken ct = default);
    Task<List<OeaRequisito>> GetRequisitosBySubseccionAsync(int subseccionId, CancellationToken ct = default);
}