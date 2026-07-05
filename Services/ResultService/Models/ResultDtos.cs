namespace ResultService.Models;

public record QuestionSummaryFromVoteService(
    int IdQuestion,
    int Si,
    int No,
    int Blanco,
    int Nulo,
    int Total
);

public record ReferendumSummaryFromVoteService(
    int IdReferendum,
    List<QuestionSummaryFromVoteService> Questions
);

public record QuestionResultResponse(
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

public record ReferendumResultResponse(
    int IdReferendum,
    int TotalVotes,
    List<QuestionResultResponse> Questions
);