namespace OeaMatriz.Domain.Enums;

/// <summary>
/// Representa el estado de cumplimiento de un requisito en la evaluación.
/// Se asigna como valor byte para facilitar el almacenamiento y el
/// procesamiento.
/// </summary>
public enum EstadoCumplimiento : byte
{
    NoEvaluado = 0,
    Cumple = 1,
    Parcial = 2,
    NoCumple = 3
}