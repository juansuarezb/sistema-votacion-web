namespace VoteService.Models;

/// <summary>
/// Representa el conteo de votos para una pregunta.
/// </summary>
/// <param name="IdQuestion">Identificador de la pregunta.</param>
/// <param name="Si">Cantidad de votos afirmativos.</param>
/// <param name="No">Cantidad de votos negativos.</param>
/// <param name="Blanco">Cantidad de votos en blanco.</param>
/// <param name="Nulo">Cantidad de votos nulos.</param>
/// <param name="Total">Cantidad total de votos registrados.</param>
public sealed record QuestionSummaryResponse(
    int IdQuestion,
    int Si,
    int No,
    int Blanco,
    int Nulo,
    int Total
);

/// <summary>
/// Representa el resumen de resultados de un referéndum.
/// </summary>
/// <param name="IdReferendum">
/// Identificador del referéndum.
/// </param>
/// <param name="Questions">
/// Resultados agregados por pregunta.
/// </param>
public sealed record ReferendumSummaryResponse(
    int IdReferendum,
    List<QuestionSummaryResponse> Questions
);