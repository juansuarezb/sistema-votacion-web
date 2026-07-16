namespace ResultService.Models;

/// <summary>
/// Representa el resumen de votos de una pregunta recibido desde VoteService.
/// </summary>
/// <param name="IdQuestion">Identificador de la pregunta.</param>
/// <param name="Si">Cantidad de votos afirmativos.</param>
/// <param name="No">Cantidad de votos negativos.</param>
/// <param name="Blanco">Cantidad de votos en blanco.</param>
/// <param name="Nulo">Cantidad de votos nulos.</param>
/// <param name="Total">Cantidad total de votos de la pregunta.</param>
public sealed record QuestionSummaryFromVoteService(
    int IdQuestion,
    int Si,
    int No,
    int Blanco,
    int Nulo,
    int Total
);

/// <summary>
/// Representa el resumen completo de un referéndum recibido desde VoteService.
/// </summary>
/// <param name="IdReferendum">Identificador del referéndum.</param>
/// <param name="Questions">Resultados agregados por pregunta.</param>
public sealed record ReferendumSummaryFromVoteService(
    int IdReferendum,
    List<QuestionSummaryFromVoteService> Questions
);

/// <summary>
/// Representa las cantidades y porcentajes calculados para una pregunta.
/// </summary>
public sealed record QuestionResultResponse(
    int IdQuestion,
    int Si,
    int No,
    int Blanco,
    int Nulo,
    int Total,
    decimal PorcentajeSi,
    decimal PorcentajeNo,
    decimal PorcentajeBlanco,
    decimal PorcentajeNulo
);

/// <summary>
/// Representa los resultados completos de un referéndum.
/// </summary>
/// <param name="IdReferendum">Identificador del referéndum.</param>
/// <param name="TotalVotes">
/// Suma de los votos registrados en todas las preguntas.
/// </param>
/// <param name="Questions">Resultados calculados por pregunta.</param>
public sealed record ReferendumResultResponse(
    int IdReferendum,
    int TotalVotes,
    List<QuestionResultResponse> Questions
);