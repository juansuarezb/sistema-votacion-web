using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReferendumService.Data;
using ReferendumService.Models;
using ReferendumService.Services;
namespace ReferendumService.Controllers;


// Expone las operaciones HTTP para administrar referéndums, preguntas,

[ApiController]
[Route("api/[controller]")]
public sealed class ReferendumsController : ControllerBase
{
    private readonly ReferendumDbContext _context;
    private readonly AuditClient _auditClient;

   
    // Inicializa una nueva instancia de ReferendumsController
  
    public ReferendumsController(
        ReferendumDbContext context,
        AuditClient auditClient)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(auditClient);

        _context = context;
        _auditClient = auditClient;
    }


    // Obtiene todos los referéndums registrados.
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


    // Obtiene un referéndum mediante su identificador.
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


    // Crea un nuevo referéndum en estado inicial BORRADOR

    [HttpPost]
    [ProducesResponseType(typeof(ReferendumResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]


    // Crea un nuevo referéndum en estado inicial <c>BORRADOR</c>.
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




    // Actualiza la información y el estado de un referéndum.
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

  
    // Elimina un referéndum y sus datos dependientes.
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
  
    // Agrega una pregunta a un referéndum existente.
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


    // Actualiza el texto de una pregunta existente en un referéndum.
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

  
    // Elimina una pregunta de un referéndum existente.
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


    // Agrega un candidato a una pregunta.
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

 
    // Actualiza un candidato existente.
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

   
    // Elimina un candidato de una pregunta.
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

  
    // Obtiene las preguntas registradas para un referéndum.
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


    // Asigna un votante a todas las preguntas de un referéndum.
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



    // Obtiene todas las asignaciones registradas para un referéndum.
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

    //Obtiene los referéndums activos que tienen preguntas pendientes para un votante determinado.

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

   
    // Evalúa si un votante puede responder una pregunta de un referéndum.
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

   
    // Marca una asignación como respondida y registra la fecha del voto.
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

  
    // Convierte una entidad de referéndum en el DTO expuesto por la API.
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
  
    // Obtiene el estado de participación de los votantes asignados a un referéndum.

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
    // Construye y envía un evento funcional hacia AuditService.
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