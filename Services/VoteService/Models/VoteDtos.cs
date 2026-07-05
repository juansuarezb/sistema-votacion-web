namespace VoteService.Models;

public record CreateVoteRequest(
    int IdReferendum,
    int IdQuestion,
    int IdVotante,
    string TipoVoto
);

public record VoteResponse(
    int IdVote,
    int IdReferendum,
    int IdQuestion,
    string TipoVoto,
    DateTime Fecha
);