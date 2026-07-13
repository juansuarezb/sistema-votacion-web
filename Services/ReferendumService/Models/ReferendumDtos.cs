namespace ReferendumService.Models;

public record CreateReferendumRequest(
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre
);

public record UpdateReferendumRequest(
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    string Estado
);

public record ReferendumResponse(
    int IdReferendum,
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    string Estado,
    DateTime FechaCreacion
);

public record CreateQuestionRequest(
    string Texto
);

public record QuestionResponse(
    int IdQuestion,
    int IdReferendum,
    string Texto
);

public record AssignVoterRequest(
    int IdVotante
);

public record EligibilityResponse(
    int IdReferendum,
    int IdQuestion,
    int IdVotante,
    bool Asignado,
    bool HaVotado,
    bool PuedeVotar,
    string Mensaje
);

public record AssignedReferendumResponse(
    int IdReferendum,
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    string Estado,
    int TotalPreguntas,
    int PreguntasPendientes
);