namespace VoteService.Models;

/// <summary>
/// Representa la respuesta de ReferendumService al comprobar si un votante
/// puede responder una pregunta.
/// </summary>
/// <param name="IdReferendum">Identificador del referéndum.</param>
/// <param name="IdQuestion">Identificador de la pregunta.</param>
/// <param name="IdVotante">Identificador interno del votante.</param>
/// <param name="Asignado">
/// Indica si el votante se encuentra asignado a la pregunta.
/// </param>
/// <param name="HaVotado">
/// Indica si la asignación ya fue marcada como respondida.
/// </param>
/// <param name="PuedeVotar">
/// Indica si el voto puede registrarse en este momento.
/// </param>
/// <param name="Mensaje">
/// Explicación del resultado de elegibilidad.
/// </param>
public sealed record EligibilityResponse(
    int IdReferendum,
    int IdQuestion,
    int IdVotante,
    bool Asignado,
    bool HaVotado,
    bool PuedeVotar,
    string Mensaje
);