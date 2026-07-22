using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReferendumService.Data;
using ReferendumService.Models;
using ReferendumService.Services;
namespace ReferendumService.Controllers;

/// <summary>
/// Expone las operaciones HTTP para administrar referéndums, preguntas,
/// asignaciones de votantes y validaciones de elegibilidad.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class ReferendumsController : ControllerBase
{
    private readonly ReferendumDbContext _context;
    private readonly AuditClient _auditClient;

    /// <summary>
    /// Inicializa una nueva instancia de
    /// <see cref="ReferendumsController"/>.
    /// </summary>
    /// <param name="context">
    /// Contexto utilizado para administrar referéndums y asignaciones.
    /// </param>
    /// <param name="auditClient">
    /// Cliente utilizado para registrar eventos de auditoría.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando una dependencia es <see langword="null"/>.
    /// </exception>
    public ReferendumsController(
        ReferendumDbContext context,
        AuditClient auditClient)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(auditClient);

        _context = context;
        _auditClient = auditClient;
    }

    /// <summary>
    /// Obtiene todos los referéndums registrados.
    /// </summary>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con la colección de referéndums registrados.
    /// La colección puede estar vacía.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<ReferendumResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<IEnumerable<ReferendumResponse>>> GetAllAsync(
        CancellationToken ct)
    {
        var referendums = await _context.Referendums
            .AsNoTracking()
            .ToListAsync(ct);

        return Ok(referendums.Select(ToResponse));
    }

    /// <summary>
    /// Obtiene un referéndum mediante su identificador.
    /// </summary>
    /// <param name="id">
    /// Identificador interno del referéndum.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con el referéndum encontrado, o 404 cuando
    /// no existe.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReferendumResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReferendumResponse>> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var referendum = await _context.Referendums
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.IdReferendum == id,
                ct
            );

        if (referendum is null)
        {
            return NotFound(new
            {
                mensaje = "Referéndum no encontrado"
            });
        }

        return Ok(ToResponse(referendum));
    }

    /// <summary>
    /// Crea un nuevo referéndum en estado inicial
    /// <c>BORRADOR</c>.
    /// </summary>
    /// <param name="request">
    /// Datos requeridos para crear el referéndum.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con el referéndum creado, o 400 cuando el rango
    /// de fechas es inválido.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// Se produce cuando SQL Server rechaza la inserción.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpPost]
    [ProducesResponseType(typeof(ReferendumResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    /// <summary>
    /// Crea un nuevo referéndum en estado inicial <c>BORRADOR</c>.
    /// </summary>
    /// <param name="request">Datos del nuevo referéndum.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>
    /// HTTP 200 con el referéndum creado o 400 cuando las fechas son inválidas.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(ReferendumResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReferendumResponse>> CreateAsync(
    CreateReferendumRequest request,
    CancellationToken ct)
    {
        if (request.FechaCierre <= request.FechaInicio)
        {
            return BadRequest(new
            {
                error = "La fecha de cierre debe ser mayor a la fecha de inicio"
            });
        }

        var referendum = new Referendum
        {
            Titulo = request.Titulo.Trim(),
            Descripcion = request.Descripcion?.Trim(),
            ImagenUrl = request.ImagenUrl?.Trim(),
            FechaInicio = request.FechaInicio,
            FechaCierre = request.FechaCierre,
            Estado = "BORRADOR"
        };

        _context.Referendums.Add(referendum);
        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync(
            action: "REFERENDUM_CREATED",
            entityType: "Referendum",
            entityId: referendum.IdReferendum.ToString(),
            httpMethod: HttpMethods.Post,
            path: "/api/referendums",
            statusCode: StatusCodes.Status200OK,
            description:
                $"Se creó el referéndum '{referendum.Titulo}'.",
            ct
        );

        return Ok(ToResponse(referendum));
    }



    /// <summary>
    /// Actualiza la información y el estado de un referéndum.
    /// </summary>
    /// <param name="id">Identificador del referéndum.</param>
    /// <param name="request">Nuevos valores.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>
    /// HTTP 204, 400 si las fechas son inválidas o 404 si no existe.
    /// </returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        UpdateReferendumRequest request,
        CancellationToken ct)
    {
        var referendum = await _context.Referendums.FindAsync(
            [id],
            ct
        );

        if (referendum is null)
        {
            return NotFound(new
            {
                mensaje = "Referéndum no encontrado"
            });
        }

        if (request.FechaCierre <= request.FechaInicio)
        {
            return BadRequest(new
            {
                error = "La fecha de cierre debe ser mayor a la fecha de inicio"
            });
        }

        var estadosPermitidos = new[]
        {
        "BORRADOR",
        "ACTIVO",
        "CERRADO",
        "CANCELADO"
    };

        var estadoNormalizado =
            request.Estado.Trim().ToUpperInvariant();

        if (!estadosPermitidos.Contains(estadoNormalizado))
        {
            return BadRequest(new
            {
                error =
                    "Estado inválido. Valores permitidos: BORRADOR, ACTIVO, CERRADO y CANCELADO."
            });
        }

        referendum.Titulo = request.Titulo.Trim();
        referendum.Descripcion = request.Descripcion?.Trim();
        referendum.ImagenUrl = request.ImagenUrl?.Trim();
        referendum.FechaInicio = request.FechaInicio;
        referendum.FechaCierre = request.FechaCierre;
        referendum.Estado = estadoNormalizado;

        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync(
            action: "REFERENDUM_UPDATED",
            entityType: "Referendum",
            entityId: id.ToString(),
            httpMethod: HttpMethods.Put,
            path: $"/api/referendums/{id}",
            statusCode: StatusCodes.Status204NoContent,
            description:
                $"Se actualizó el referéndum '{referendum.Titulo}' y quedó en estado {referendum.Estado}.",
            ct
        );

        return NoContent();
    }

    /// <summary>
    /// Elimina un referéndum y sus datos dependientes.
    /// </summary>
    /// <param name="id">Identificador del referéndum.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>
    /// HTTP 204 cuando se elimina o 404 cuando no existe.
    /// </returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int id,
        CancellationToken ct)
    {
        var referendum = await _context.Referendums.FindAsync(
            [id],
            ct
        );

        if (referendum is null)
        {
            return NotFound(new
            {
                mensaje = "Referéndum no encontrado"
            });
        }

        var tituloEliminado = referendum.Titulo;

        _context.Referendums.Remove(referendum);
        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync(
            action: "REFERENDUM_DELETED",
            entityType: "Referendum",
            entityId: id.ToString(),
            httpMethod: HttpMethods.Delete,
            path: $"/api/referendums/{id}",
            statusCode: StatusCodes.Status204NoContent,
            description:
                $"Se eliminó el referéndum '{tituloEliminado}'.",
            ct
        );

        return NoContent();
    }
    /// <summary>
    /// Agrega una pregunta a un referéndum existente.
    /// </summary>
    /// <param name="id">Identificador del referéndum.</param>
    /// <param name="request">Texto de la pregunta.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>
    /// HTTP 200 con la pregunta creada, 400 si el texto está vacío o 404 si el
    /// referéndum no existe.
    /// </returns>
    [HttpPost("{id:int}/questions")]
    [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuestionResponse>> AddQuestionAsync(
        int id,
        CreateQuestionRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
        {
            return BadRequest(new
            {
                error = "El texto de la pregunta es obligatorio."
            });
        }

        var referendumExists = await _context.Referendums
            .AnyAsync(
                item => item.IdReferendum == id,
                ct
            );

        if (!referendumExists)
        {
            return NotFound(new
            {
                mensaje = "Referéndum no encontrado"
            });
        }

        var question = new ReferendumQuestion
        {
            IdReferendum = id,
            Texto = request.Texto.Trim()
        };

        _context.ReferendumQuestions.Add(question);
        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync(
            action: "QUESTION_CREATED",
            entityType: "ReferendumQuestion",
            entityId: question.IdQuestion.ToString(),
            httpMethod: HttpMethods.Post,
            path: $"/api/referendums/{id}/questions",
            statusCode: StatusCodes.Status200OK,
            description:
                $"Se agregó la pregunta {question.IdQuestion} al referéndum {id}.",
            ct
        );

        return Ok(new QuestionResponse(
            question.IdQuestion,
            question.IdReferendum,
            question.Texto,
            Enumerable.Empty<CandidateResponse>()
        ));
    }

    /// <summary>
    /// Actualiza el texto de una pregunta existente en un referéndum.
    /// </summary>
    /// <param name="id">Identificador del referéndum.</param>
    /// <param name="idQuestion">Identificador de la pregunta.</param>
    /// <param name="request">Nuevo texto de la pregunta.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>HTTP 204 si es exitoso, 400 si el texto es inválido, 404 si no existe.</returns>
    [HttpPut("{id:int}/questions/{idQuestion:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuestionAsync(
        int id,
        int idQuestion,
        UpdateQuestionRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
        {
            return BadRequest(new { error = "El texto de la pregunta es obligatorio." });
        }

        var question = await _context.ReferendumQuestions
            .FirstOrDefaultAsync(q => q.IdReferendum == id && q.IdQuestion == idQuestion, ct);

        if (question is null)
        {
            return NotFound(new { mensaje = "Pregunta no encontrada en este referéndum." });
        }

        question.Texto = request.Texto.Trim();
        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync(
            action: "QUESTION_UPDATED",
            entityType: "ReferendumQuestion",
            entityId: question.IdQuestion.ToString(),
            httpMethod: HttpMethods.Put,
            path: $"/api/referendums/{id}/questions/{idQuestion}",
            statusCode: StatusCodes.Status204NoContent,
            description: $"Se actualizó el texto de la pregunta {question.IdQuestion} del referéndum {id}.",
            ct
        );

        return NoContent();
    }

    /// <summary>
    /// Elimina una pregunta de un referéndum existente.
    /// </summary>
    /// <param name="id">Identificador del referéndum.</param>
    /// <param name="idQuestion">Identificador de la pregunta.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>HTTP 204 si es exitoso, 404 si no existe.</returns>
    [HttpDelete("{id:int}/questions/{idQuestion:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestionAsync(
        int id,
        int idQuestion,
        CancellationToken ct)
    {
        var question = await _context.ReferendumQuestions
            .FirstOrDefaultAsync(q => q.IdReferendum == id && q.IdQuestion == idQuestion, ct);

        if (question is null)
        {
            return NotFound(new { mensaje = "Pregunta no encontrada en este referéndum." });
        }

        var voters = await _context.ReferendumQuestionVoters
            .Where(v => v.IdReferendum == id && v.IdQuestion == idQuestion)
            .ToListAsync(ct);

        if (voters.Any())
        {
            _context.ReferendumQuestionVoters.RemoveRange(voters);
        }

        _context.ReferendumQuestions.Remove(question);
        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync(
            action: "QUESTION_DELETED",
            entityType: "ReferendumQuestion",
            entityId: idQuestion.ToString(),
            httpMethod: HttpMethods.Delete,
            path: $"/api/referendums/{id}/questions/{idQuestion}",
            statusCode: StatusCodes.Status204NoContent,
            description: $"Se eliminó la pregunta {idQuestion} del referéndum {id}.",
            ct
        );

        return NoContent();
    }

    /// <summary>
    /// Agrega un candidato a una pregunta.
    /// </summary>
    [HttpPost("{id:int}/questions/{idQuestion:int}/candidates")]
    [ProducesResponseType(typeof(CandidateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CandidateResponse>> AddCandidateAsync(
        int id,
        int idQuestion,
        CreateCandidateRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { error = "El nombre del candidato es obligatorio." });

        var question = await _context.ReferendumQuestions
            .FirstOrDefaultAsync(q => q.IdReferendum == id && q.IdQuestion == idQuestion, ct);

        if (question is null)
            return NotFound(new { mensaje = "Pregunta no encontrada." });

        var candidate = new ReferendumQuestionCandidate
        {
            IdQuestion = idQuestion,
            Nombre = request.Nombre.Trim(),
            ImagenUrl = request.ImagenUrl?.Trim()
        };

        _context.ReferendumQuestionCandidates.Add(candidate);
        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync("CANDIDATE_CREATED", "ReferendumQuestionCandidate", candidate.IdCandidate.ToString(),
            HttpMethods.Post, $"/api/referendums/{id}/questions/{idQuestion}/candidates", StatusCodes.Status200OK,
            $"Candidato {candidate.IdCandidate} agregado a la pregunta {idQuestion}.", ct);

        return Ok(new CandidateResponse(candidate.IdCandidate, candidate.IdQuestion, candidate.Nombre, candidate.ImagenUrl));
    }

    /// <summary>
    /// Actualiza un candidato existente.
    /// </summary>
    [HttpPut("{id:int}/questions/{idQuestion:int}/candidates/{idCandidate:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCandidateAsync(
        int id,
        int idQuestion,
        int idCandidate,
        CreateCandidateRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { error = "El nombre del candidato es obligatorio." });

        var candidate = await _context.ReferendumQuestionCandidates
            .FirstOrDefaultAsync(c => c.IdQuestion == idQuestion && c.IdCandidate == idCandidate, ct);

        if (candidate is null)
            return NotFound(new { mensaje = "Candidato no encontrado." });

        candidate.Nombre = request.Nombre.Trim();
        candidate.ImagenUrl = request.ImagenUrl?.Trim();

        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync("CANDIDATE_UPDATED", "ReferendumQuestionCandidate", idCandidate.ToString(),
            HttpMethods.Put, $"/api/referendums/{id}/questions/{idQuestion}/candidates/{idCandidate}", StatusCodes.Status204NoContent,
            $"Candidato {idCandidate} actualizado en la pregunta {idQuestion}.", ct);

        return NoContent();
    }

    /// <summary>
    /// Elimina un candidato de una pregunta.
    /// </summary>
    [HttpDelete("{id:int}/questions/{idQuestion:int}/candidates/{idCandidate:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCandidateAsync(
        int id,
        int idQuestion,
        int idCandidate,
        CancellationToken ct)
    {
        var candidate = await _context.ReferendumQuestionCandidates
            .FirstOrDefaultAsync(c => c.IdQuestion == idQuestion && c.IdCandidate == idCandidate, ct);

        if (candidate is null)
            return NotFound(new { mensaje = "Candidato no encontrado." });

        _context.ReferendumQuestionCandidates.Remove(candidate);
        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync("CANDIDATE_DELETED", "ReferendumQuestionCandidate", idCandidate.ToString(),
            HttpMethods.Delete, $"/api/referendums/{id}/questions/{idQuestion}/candidates/{idCandidate}", StatusCodes.Status204NoContent,
            $"Candidato {idCandidate} eliminado de la pregunta {idQuestion}.", ct);

        return NoContent();
    }

    /// <summary>
    /// Obtiene las preguntas registradas para un referéndum.
    /// </summary>
    /// <param name="id">
    /// Identificador del referéndum.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con las preguntas encontradas. La colección
    /// puede estar vacía.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpGet("{id:int}/questions")]
    [ProducesResponseType(
        typeof(IEnumerable<QuestionResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetQuestionsAsync(
        int id,
        CancellationToken ct)
    {
        var questions = await _context.ReferendumQuestions
            .Include(q => q.Candidatos)
            .AsNoTracking()
            .Where(question => question.IdReferendum == id)
            .ToListAsync(ct);

        return Ok(questions.Select(question => new QuestionResponse(
            question.IdQuestion,
            question.IdReferendum,
            question.Texto,
            question.Candidatos.Select(c => new CandidateResponse(c.IdCandidate, c.IdQuestion, c.Nombre, c.ImagenUrl))
        )));
    }

    /// <summary>
    /// Asigna un votante a todas las preguntas de un referéndum.
    /// </summary>
    /// <param name="id">Identificador del referéndum.</param>
    /// <param name="request">Identificador del votante.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>
    /// HTTP 200 cuando se asigna, 400 para datos inválidos, 404 si no existe el
    /// referéndum o 409 cuando el votante ya fue asignado.
    /// </returns>
    [HttpPost("{id:int}/voters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignVoterAsync(
        int id,
        AssignVoterRequest request,
        CancellationToken ct)
    {
        if (request.IdVotante <= 0)
        {
            return BadRequest(new
            {
                error = "El identificador del votante no es válido."
            });
        }

        var referendum = await _context.Referendums
            .Include(item => item.Preguntas)
            .FirstOrDefaultAsync(
                item => item.IdReferendum == id,
                ct
            );

        if (referendum is null)
        {
            return NotFound(new
            {
                mensaje = "Referéndum no encontrado"
            });
        }

        if (referendum.Preguntas.Count == 0)
        {
            return BadRequest(new
            {
                error = "El referéndum no tiene preguntas registradas."
            });
        }

        var asignacionesExistentes =
            await _context.ReferendumQuestionVoters
                .AsNoTracking()
                .Where(assignment =>
                    assignment.IdReferendum == id &&
                    assignment.IdVotante == request.IdVotante
                )
                .ToListAsync(ct);

        if (asignacionesExistentes.Count > 0)
        {
            var preguntasRespondidas =
                asignacionesExistentes.Count(
                    assignment => assignment.HaVotado
                );

            var haCompletado =
                asignacionesExistentes.Count ==
                referendum.Preguntas.Count &&
                preguntasRespondidas ==
                referendum.Preguntas.Count;

            if (haCompletado)
            {
                return Conflict(new
                {
                    error = "El votante ya completó esta votación."
                });
            }

            return Conflict(new
            {
                error = "El votante ya está asignado a esta votación."
            });
        }

        foreach (var question in referendum.Preguntas)
        {
            _context.ReferendumQuestionVoters.Add(
                new ReferendumQuestionVoter
                {
                    IdReferendum = id,
                    IdQuestion = question.IdQuestion,
                    IdVotante = request.IdVotante,
                    HaVotado = false
                }
            );
        }

        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync(
            action: "VOTER_ASSIGNED",
            entityType: "Referendum",
            entityId: id.ToString(),
            httpMethod: HttpMethods.Post,
            path: $"/api/referendums/{id}/voters",
            statusCode: StatusCodes.Status200OK,
            description:
                $"Se asignó el votante {request.IdVotante} a las {referendum.Preguntas.Count} preguntas del referéndum {id}.",
            ct
        );

        return Ok(new
        {
            mensaje =
                "Votante asignado a todas las preguntas del referéndum."
        });

        // TODO: Validar mediante VoterService que IdVotante corresponda a un
        // perfil existente antes de persistir las asignaciones.
    }


    /// <summary>
    /// Obtiene todas las asignaciones registradas para un referéndum.
    /// </summary>
    /// <param name="id">
    /// Identificador del referéndum.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con las asignaciones ordenadas por votante y
    /// pregunta.
    /// </returns>
    [HttpGet("{id:int}/voters")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignedVotersAsync(
        int id,
        CancellationToken ct)
    {
        var voters = await _context.ReferendumQuestionVoters
            .AsNoTracking()
            .Where(assignment => assignment.IdReferendum == id)
            .OrderBy(assignment => assignment.IdVotante)
            .ThenBy(assignment => assignment.IdQuestion)
            .ToListAsync(ct);

        return Ok(voters);
    }

    /// <summary>
    /// Obtiene los referéndums activos que tienen preguntas pendientes para
    /// un votante determinado.
    /// </summary>
    /// <param name="idVotante">
    /// Identificador interno del perfil electoral.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con los referéndums disponibles o 400 cuando el
    /// identificador del votante no es válido.
    /// </returns>
    [HttpGet("voters/{idVotante:int}/assigned")]
    [ProducesResponseType(
        typeof(IEnumerable<AssignedReferendumResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AssignedReferendumResponse>>>
        GetAssignedReferendumsByVoterAsync(
            int idVotante,
            CancellationToken ct)
    {
        if (idVotante <= 0)
        {
            return BadRequest(new
            {
                error = "El identificador del votante no es válido."
            });
        }

        var now = DateTime.UtcNow;

        var assignedReferendumIds =
            await _context.ReferendumQuestionVoters
                .AsNoTracking()
                .Where(assignment =>
                    assignment.IdVotante == idVotante)
                .Select(assignment =>
                    assignment.IdReferendum)
                .Distinct()
                .ToListAsync(ct);

        if (assignedReferendumIds.Count == 0)
        {
            return Ok(Array.Empty<AssignedReferendumResponse>());
        }

        var referendums = await _context.Referendums
            .AsNoTracking()
            .Where(referendum =>
                assignedReferendumIds.Contains(
                    referendum.IdReferendum
                ) &&
                referendum.Estado == "ACTIVO" &&
                referendum.FechaInicio <= now &&
                referendum.FechaCierre >= now
            )
            .OrderBy(referendum => referendum.FechaCierre)
            .ToListAsync(ct);

        var response = new List<AssignedReferendumResponse>();

        foreach (var referendum in referendums)
        {
            var assignments =
                await _context.ReferendumQuestionVoters
                    .AsNoTracking()
                    .Where(assignment =>
                        assignment.IdReferendum ==
                        referendum.IdReferendum &&
                        assignment.IdVotante == idVotante
                    )
                    .ToListAsync(ct);

            var totalPreguntas = assignments.Count;
            var preguntasPendientes =
                assignments.Count(assignment => !assignment.HaVotado);

            // Se eliminó la validación (preguntasPendientes == 0) para permitir mostrar
            // las votaciones completadas en el historial del frontend.

            response.Add(
                new AssignedReferendumResponse(
                    referendum.IdReferendum,
                    referendum.Titulo,
                    referendum.Descripcion,
                    referendum.ImagenUrl,
                    referendum.FechaInicio,
                    referendum.FechaCierre,
                    referendum.Estado,
                    totalPreguntas,
                    preguntasPendientes
                )
            );
        }

        // TODO: La consulta actual realiza una consulta adicional por cada
        // referéndum. Puede optimizarse con una proyección agrupada cuando
        // se confirme una traducción estable del proveedor SQL Server.
        return Ok(response);
    }

    /// <summary>
    /// Evalúa si un votante puede responder una pregunta de un referéndum.
    /// </summary>
    /// <param name="id">
    /// Identificador del referéndum.
    /// </param>
    /// <param name="idQuestion">
    /// Identificador de la pregunta.
    /// </param>
    /// <param name="idVotante">
    /// Identificador interno del votante.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con el estado de elegibilidad o 404 cuando el
    /// referéndum o la pregunta no existen.
    /// </returns>
    [HttpGet(
        "{id:int}/questions/{idQuestion:int}/voters/{idVotante:int}/eligibility"
    )]
    [ProducesResponseType(typeof(EligibilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EligibilityResponse>> CheckEligibilityAsync(
        int id,
        int idQuestion,
        int idVotante,
        CancellationToken ct)
    {
        var referendum = await _context.Referendums
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.IdReferendum == id,
                ct
            );

        if (referendum is null)
        {
            return NotFound(new
            {
                mensaje = "Referéndum no encontrado"
            });
        }

        var questionExists =
            await _context.ReferendumQuestions.AnyAsync(
                question =>
                    question.IdReferendum == id &&
                    question.IdQuestion == idQuestion,
                ct
            );

        if (!questionExists)
        {
            return NotFound(new
            {
                mensaje = "Pregunta no encontrada para este referéndum"
            });
        }

        var assignment =
            await _context.ReferendumQuestionVoters
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item =>
                        item.IdReferendum == id &&
                        item.IdQuestion == idQuestion &&
                        item.IdVotante == idVotante,
                    ct
                );

        if (assignment is null)
        {
            return Ok(new EligibilityResponse(
                id,
                idQuestion,
                idVotante,
                false,
                false,
                false,
                "El votante no está asignado a esta pregunta"
            ));
        }

        var now = DateTime.UtcNow;

        if (referendum.Estado != "ACTIVO")
        {
            return Ok(new EligibilityResponse(
                id,
                idQuestion,
                idVotante,
                true,
                assignment.HaVotado,
                false,
                "El referéndum no está activo"
            ));
        }

        if (now < referendum.FechaInicio ||
            now > referendum.FechaCierre)
        {
            return Ok(new EligibilityResponse(
                id,
                idQuestion,
                idVotante,
                true,
                assignment.HaVotado,
                false,
                "El referéndum está fuera del periodo permitido"
            ));
        }

        if (assignment.HaVotado)
        {
            return Ok(new EligibilityResponse(
                id,
                idQuestion,
                idVotante,
                true,
                true,
                false,
                "El votante ya respondió esta pregunta"
            ));
        }

        return Ok(new EligibilityResponse(
            id,
            idQuestion,
            idVotante,
            true,
            false,
            true,
            "El votante puede responder esta pregunta"
        ));
    }

    /// <summary>
    /// Marca una asignación como respondida y registra la fecha del voto.
    /// </summary>
    /// <param name="id">
    /// Identificador del referéndum.
    /// </param>
    /// <param name="idQuestion">
    /// Identificador de la pregunta respondida.
    /// </param>
    /// <param name="idVotante">
    /// Identificador interno del votante.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 204 cuando se actualiza correctamente, 404 cuando
    /// no existe la asignación o 409 cuando ya había sido respondida.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// Se produce cuando SQL Server rechaza la actualización.
    /// </exception>
    [HttpPatch(
        "{id:int}/questions/{idQuestion:int}/voters/{idVotante:int}/mark-voted"
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkVotedAsync(
        int id,
        int idQuestion,
        int idVotante,
        CancellationToken ct)
    {
        var assignment =
            await _context.ReferendumQuestionVoters
                .FirstOrDefaultAsync(
                    item =>
                        item.IdReferendum == id &&
                        item.IdQuestion == idQuestion &&
                        item.IdVotante == idVotante,
                    ct
                );

        if (assignment is null)
        {
            return NotFound(new
            {
                mensaje = "Asignación no encontrada"
            });
        }

        if (assignment.HaVotado)
        {
            return Conflict(new
            {
                error = "El votante ya respondió esta pregunta"
            });
        }

        assignment.HaVotado = true;
        assignment.FechaVoto = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return NoContent();

        // FIXME: La comprobación y actualización no son atómicas frente a
        // dos solicitudes simultáneas. Debe reforzarse mediante concurrencia
        // optimista o una actualización condicional en la base de datos.
    }

    /// <summary>
    /// Convierte una entidad de referéndum en el DTO expuesto por la API.
    /// </summary>
    /// <param name="referendum">
    /// Entidad que debe convertirse.
    /// </param>
    /// <returns>
    /// Una instancia de <see cref="ReferendumResponse"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando <paramref name="referendum"/> es
    /// <see langword="null"/>.
    /// </exception>
    private static ReferendumResponse ToResponse(
        Referendum referendum)
    {
        ArgumentNullException.ThrowIfNull(referendum);

        return new ReferendumResponse(
            referendum.IdReferendum,
            referendum.Titulo,
            referendum.Descripcion,
            referendum.ImagenUrl,
            referendum.FechaInicio,
            referendum.FechaCierre,
            referendum.Estado,
            referendum.FechaCreacion
        );
    }
    /// <summary>
    /// Obtiene el estado de participación de los votantes asignados a un
    /// referéndum.
    /// </summary>
    /// <param name="id">
    /// Identificador del referéndum.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con el estado agrupado por votante o 404 cuando el
    /// referéndum no existe.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación se cancela mediante <paramref name="ct"/>.
    /// </exception>
    [HttpGet("{id:int}/voters/status")]
    [ProducesResponseType(
        typeof(IEnumerable<VoterAssignmentStatusResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<VoterAssignmentStatusResponse>>>
        GetVoterAssignmentStatusesAsync(
            int id,
            CancellationToken ct)
    {
        var referendum = await _context.Referendums
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.IdReferendum == id,
                ct
            );

        if (referendum is null)
        {
            return NotFound(new
            {
                mensaje = "Referéndum no encontrado"
            });
        }

        var totalPreguntas = await _context.ReferendumQuestions
            .AsNoTracking()
            .CountAsync(
                question => question.IdReferendum == id,
                ct
            );

        var assignments = await _context.ReferendumQuestionVoters
            .AsNoTracking()
            .Where(assignment => assignment.IdReferendum == id)
            .ToListAsync(ct);

        var response = assignments
            .GroupBy(assignment => assignment.IdVotante)
            .Select(group =>
            {
                var preguntasRespondidas =
                    group.Count(assignment => assignment.HaVotado);

                var preguntasAsignadas = group.Count();

                var haCompletado =
                    totalPreguntas > 0 &&
                    preguntasAsignadas == totalPreguntas &&
                    preguntasRespondidas == totalPreguntas;

                var preguntasPendientes = Math.Max(
                    totalPreguntas - preguntasRespondidas,
                    0
                );

                return new VoterAssignmentStatusResponse(
                    group.Key,
                    true,
                    haCompletado,
                    totalPreguntas,
                    preguntasRespondidas,
                    preguntasPendientes,
                    haCompletado ? "COMPLETADO" : "ASIGNADO"
                );
            })
            .OrderBy(status => status.IdVotante)
            .ToList();

        return Ok(response);
    }
    /// <summary>
    /// Construye y envía un evento funcional hacia AuditService.
    /// </summary>
    /// <param name="action">Acción funcional realizada.</param>
    /// <param name="entityType">Tipo de entidad afectada.</param>
    /// <param name="entityId">Identificador de la entidad.</param>
    /// <param name="httpMethod">Método HTTP asociado.</param>
    /// <param name="path">Ruta HTTP procesada.</param>
    /// <param name="statusCode">Código HTTP resultante.</param>
    /// <param name="description">Descripción del evento.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Una tarea que representa el intento de auditoría.</returns>
    private async Task WriteAuditEventAsync(
        string action,
        string entityType,
        string? entityId,
        string httpMethod,
        string path,
        int statusCode,
        string description,
        CancellationToken ct)
    {
        var userId =
            User.FindFirst("sub")?.Value;

        var username =
            User.Identity?.Name ??
            User.FindFirst("preferred_username")?.Value;

        await _auditClient.TryWriteAsync(
            new CreateAuditEventRequest(
                ServiceName: "ReferendumService",
                Action: action,
                EntityType: entityType,
                EntityId: entityId,
                UserId: userId,
                Username: username,
                HttpMethod: httpMethod,
                Path: path,
                StatusCode: statusCode,
                Success: true,
                Description: description,
                IpAddress:
                    HttpContext.Connection.RemoteIpAddress?.ToString()
            ),
            ct
        );
    }
}