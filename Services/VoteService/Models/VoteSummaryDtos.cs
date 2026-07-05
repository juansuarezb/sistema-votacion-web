namespace VoteService.Models;

public record QuestionSummaryResponse(
    int IdQuestion,
    int Si,
    int No,
    int Blanco,
    int Nulo,
    int Total
);

public record ReferendumSummaryResponse(
    int IdReferendum,
    List<QuestionSummaryResponse> Questions
);