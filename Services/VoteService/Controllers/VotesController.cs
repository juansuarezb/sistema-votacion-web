using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoteService.Data;
using VoteService.Models;
using VoteService.Services;

namespace VoteService.Controllers;

// Expone las operaciones HTTP para registrar votos, consultar votos
// almacenados y obtener resultados agregados por referéndum.
[ApiController]
[Route("api/[controller]")]
public sealed class VotesController : ControllerBase
{


    private readonly VoteDbContext _context;
    private readonly ReferendumClient _referendumClient;
    private readonly AuditClient _auditClient;

   
    // Inicializa una nueva instancia de <see cref="VotesController"/>.
    public VotesController(
        VoteDbContext context,
        ReferendumClient referendumClient,
        AuditClient auditClient)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(referendumClient);
        ArgumentNullException.ThrowIfNull(auditClient);

        _context = context;
        _referendumClient = referendumClient;
        _auditClient = auditClient;
    }

  
    // Obtiene todos los votos registrados.
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<VoteResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<IEnumerable<VoteResponse>>> GetAllAsync(
        CancellationToken ct)
    {
        var votes = await _context.Votes
            .AsNoTracking()
            .ToListAsync(ct);

        return Ok(votes.Select(ToResponse));
    }

    
    // Obtiene un voto mediante su identificador interno.
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(VoteResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VoteResponse>> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var vote = await _context.Votes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.IdVote == id,
                ct
            );

        if (vote is null)
        {
            return NotFound(new
            {
                mensaje = "Voto no encontrado"
            });
        }

        return Ok(ToResponse(vote));
    }

  
    // Obtiene un resumen agregado de votos por pregunta para un referéndum.
    [HttpGet("referendums/{idReferendum:int}/summary")]
    [ProducesResponseType(
        typeof(ReferendumSummaryResponse),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<ReferendumSummaryResponse>>
        GetSummaryAsync(
            int idReferendum,
            CancellationToken ct)
    {
        var votes = await _context.Votes
            .AsNoTracking()
            .Where(vote =>
                vote.IdReferendum == idReferendum
            )
            .ToListAsync(ct);

        var summary = votes
            .GroupBy(vote => vote.IdQuestion)
            .Select(group =>
                new QuestionSummaryResponse(
                    group.Key,
                    group.Count(vote =>
                        vote.TipoVoto == "SI"),
                    group.Count(vote =>
                        vote.TipoVoto == "NO"),
                    group.Count(vote =>
                        vote.TipoVoto == "BLANCO"),
                    group.Count(vote =>
                        vote.TipoVoto == "NULO"),
                    group.Count()
                )
            )
            .OrderBy(question =>
                question.IdQuestion
            )
            .ToList();

        return Ok(
            new ReferendumSummaryResponse(
                idReferendum,
                summary
            )
        );
    }

    
    // Registra un voto después de validar el tipo de voto y comprobar la
    // elegibilidad del votante mediante ReferendumService.
    [HttpPost]
    [ProducesResponseType(
        typeof(VoteResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError
    )]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<VoteResponse>> CreateAsync(
        CreateVoteRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.TipoVoto))
        {
            return BadRequest(new
            {
                error = "El tipo de voto es obligatorio."
            });
        }

        var tipoVoto = request.TipoVoto
            .Trim()
            .ToUpperInvariant();

        var eligibility =
            await _referendumClient.CheckEligibilityAsync(
                request.IdReferendum,
                request.IdQuestion,
                request.IdVotante,
                ct
            );

        if (eligibility is null)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new
                {
                    error =
                        "No se pudo consultar la elegibilidad del votante."
                }
            );
        }

        if (!eligibility.PuedeVotar)
        {
            return Conflict(new
            {
                error = eligibility.Mensaje
            });
        }

        var vote = new Vote
        {
            IdReferendum = request.IdReferendum,
            IdQuestion = request.IdQuestion,
            TipoVoto = tipoVoto,
            Fecha = DateTime.UtcNow
        };

        _context.Votes.Add(vote);
        await _context.SaveChangesAsync(ct);

        /*
         * La operación distribuida no es atómica: el voto se almacena en
         * VoteService y posteriormente se actualiza la asignación en
         * ReferendumService.
         *
         * FIXME: Implementar idempotencia, outbox o reconciliación antes de
         * utilizar este diseño en producción.
         */
        var marked =
            await _referendumClient.MarkVotedAsync(
                request.IdReferendum,
                request.IdQuestion,
                request.IdVotante,
                ct
            );

        if (!marked)
        {
            await _auditClient.TryWriteAsync(
                new CreateAuditEventRequest(
                    ServiceName: "VoteService",
                    Action: "VOTE_MARK_FAILED",
                    EntityType: "ReferendumQuestion",
                    EntityId:
                        $"{request.IdReferendum}:{request.IdQuestion}",
                    UserId: null,
                    Username: null,
                    HttpMethod: HttpMethods.Post,
                    Path: "/api/votes",
                    StatusCode:
                        StatusCodes.Status500InternalServerError,
                    Success: false,
                    Description:
                        "El voto fue almacenado, pero no fue posible actualizar la asignación en ReferendumService.",
                    IpAddress:
                        HttpContext.Connection
                            .RemoteIpAddress?
                            .ToString()
                ),
                ct
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error =
                        "El voto fue registrado, pero no se pudo marcar al votante como que ya votó."
                }
            );
        }

        /*
         * Por secreto del voto no se registra:
         * - TipoVoto
         * - IdVotante
         * - IdVote
         *
         * El evento demuestra únicamente que se registró una respuesta para
         * una pregunta determinada.
         */
        await _auditClient.TryWriteAsync(
            new CreateAuditEventRequest(
                ServiceName: "VoteService",
                Action: "VOTE_CAST",
                EntityType: "ReferendumQuestion",
                EntityId:
                    $"{request.IdReferendum}:{request.IdQuestion}",
                UserId: null,
                Username: null,
                HttpMethod: HttpMethods.Post,
                Path: "/api/votes",
                StatusCode: StatusCodes.Status200OK,
                Success: true,
                Description:
                    $"Se registró una respuesta para la pregunta " +
                    $"{request.IdQuestion} del referéndum " +
                    $"{request.IdReferendum}.",
                IpAddress:
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString()
            ),
            ct
        );

        return Ok(ToResponse(vote));
    }

    
    // Obtiene todos los votos asociados a un referéndum.s votos encontrados.
    [HttpGet("referendums/{idReferendum:int}")]
    [ProducesResponseType(
        typeof(IEnumerable<VoteResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<IEnumerable<VoteResponse>>>
        GetByReferendumAsync(
            int idReferendum,
            CancellationToken ct)
    {
        var votes = await _context.Votes
            .AsNoTracking()
            .Where(vote =>
                vote.IdReferendum == idReferendum
            )
            .ToListAsync(ct);

        return Ok(votes.Select(ToResponse));
    }

  
    // Convierte una entidad persistida en el DTO expuesto por la API.
    private static VoteResponse ToResponse(Vote vote)
    {
        ArgumentNullException.ThrowIfNull(vote);

        return new VoteResponse(
            vote.IdVote,
            vote.IdReferendum,
            vote.IdQuestion,
            vote.TipoVoto,
            vote.Fecha
        );
    }
}