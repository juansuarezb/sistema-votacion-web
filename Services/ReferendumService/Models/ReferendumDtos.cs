namespace ReferendumService.Models;

/// <summary>
/// Representa los datos requeridos para crear un referéndum.
/// </summary>
/// <param name="Titulo">Título del referéndum.</param>
/// <param name="Descripcion">Descripción opcional.</param>
/// <param name="FechaInicio">Fecha UTC de inicio.</param>
/// <param name="FechaCierre">Fecha UTC de cierre.</param>
public sealed record CreateReferendumRequest(
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre
);

/// <summary>
/// Representa los datos modificables de un referéndum.
/// </summary>
/// <param name="Titulo">Nuevo título.</param>
/// <param name="Descripcion">Nueva descripción opcional.</param>
/// <param name="FechaInicio">Nueva fecha UTC de inicio.</param>
/// <param name="FechaCierre">Nueva fecha UTC de cierre.</param>
/// <param name="Estado">Nuevo estado operativo.</param>
public sealed record UpdateReferendumRequest(
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    string Estado
);

/// <summary>
/// Representa la información de un referéndum expuesta por la API.
/// </summary>
public sealed record ReferendumResponse(
    int IdReferendum,
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    string Estado,
    DateTime FechaCreacion
);

/// <summary>
/// Representa la solicitud de creación de una pregunta.
/// </summary>
/// <param name="Texto">Contenido textual de la pregunta.</param>
public sealed record CreateQuestionRequest(
    string Texto
);

/// <summary>
/// Representa una pregunta devuelta por la API.
/// </summary>
public sealed record QuestionResponse(
    int IdQuestion,
    int IdReferendum,
    string Texto
);

/// <summary>
/// Representa la solicitud de asignación de un votante.
/// </summary>
/// <param name="IdVotante">
/// Identificador interno del perfil en VoterService.
/// </param>
public sealed record AssignVoterRequest(
    int IdVotante
);

/// <summary>
/// Representa el resultado de la validación de elegibilidad.
/// </summary>
public sealed record EligibilityResponse(
    int IdReferendum,
    int IdQuestion,
    int IdVotante,
    bool Asignado,
    bool HaVotado,
    bool PuedeVotar,
    string Mensaje
);

/// <summary>
/// Representa un referéndum asignado al votante y el estado de sus preguntas.
/// </summary>
public sealed record AssignedReferendumResponse(
    int IdReferendum,
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    string Estado,
    int TotalPreguntas,
    int PreguntasPendientes
);

/// <summary>
/// Representa el estado de asignación y participación de un votante dentro
/// de un referéndum.
/// </summary>
/// <param name="IdVotante">
/// Identificador interno del votante.
/// </param>
/// <param name="Asignado">
/// Indica si el votante tiene asignaciones en el referéndum.
/// </param>
/// <param name="HaCompletado">
/// Indica si el votante respondió todas las preguntas asignadas.
/// </param>
/// <param name="TotalPreguntas">
/// Cantidad total de preguntas existentes en el referéndum.
/// </param>
/// <param name="PreguntasRespondidas">
/// Cantidad de preguntas que ya fueron respondidas.
/// </param>
/// <param name="PreguntasPendientes">
/// Cantidad de preguntas pendientes de respuesta.
/// </param>
/// <param name="Estado">
/// Estado legible de la participación: DISPONIBLE, ASIGNADO o COMPLETADO.
/// </param>
public sealed record VoterAssignmentStatusResponse(
    int IdVotante,
    bool Asignado,
    bool HaCompletado,
    int TotalPreguntas,
    int PreguntasRespondidas,
    int PreguntasPendientes,
    string Estado
);