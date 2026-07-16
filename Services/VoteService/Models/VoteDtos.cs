namespace VoteService.Models;

/// <summary>
/// Representa los datos requeridos para emitir un voto.
/// </summary>
/// <param name="IdReferendum">
/// Identificador del referéndum.
/// </param>
/// <param name="IdQuestion">
/// Identificador de la pregunta.
/// </param>
/// <param name="IdVotante">
/// Identificador interno del votante utilizado exclusivamente para validar
/// elegibilidad. No se persiste junto al contenido del voto.
/// </param>
/// <param name="TipoVoto">
/// Selección emitida: SI, NO, BLANCO o NULO.
/// </param>
public sealed record CreateVoteRequest(
    int IdReferendum,
    int IdQuestion,
    int IdVotante,
    string TipoVoto
);

/// <summary>
/// Representa un voto devuelto por la API.
/// </summary>
/// <param name="IdVote">Identificador interno del voto.</param>
/// <param name="IdReferendum">Identificador del referéndum.</param>
/// <param name="IdQuestion">Identificador de la pregunta.</param>
/// <param name="TipoVoto">Tipo de voto registrado.</param>
/// <param name="Fecha">Fecha UTC de registro.</param>
public sealed record VoteResponse(
    int IdVote,
    int IdReferendum,
    int IdQuestion,
    string TipoVoto,
    DateTime Fecha
);