using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoteService.Data;
using VoteService.Models;
using VoteService.Services;

namespace VoteService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VotesController : ControllerBase
{
    private readonly VoteDbContext _context;
    private readonly ReferendumClient _referendumClient;

    private static readonly string[] TiposPermitidos =
    {
        "SI", "NO", "BLANCO", "NULO"
    };

    public VotesController(
        VoteDbContext context,
        ReferendumClient referendumClient)
    {
        _context = context;
        _referendumClient = referendumClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VoteResponse>>> GetAllAsync(CancellationToken ct)
    {
        var votes = await _context.Votes
            .AsNoTracking()
            .ToListAsync(ct);

        return Ok(votes.Select(v => new VoteResponse(
            v.IdVote,
            v.IdReferendum,
            v.IdQuestion,
            v.TipoVoto,
            v.Fecha
        )));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VoteResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var vote = await _context.Votes
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.IdVote == id, ct);

        if (vote is null)
            return NotFound(new { mensaje = "Voto no encontrado" });

        return Ok(new VoteResponse(
            vote.IdVote,
            vote.IdReferendum,
            vote.IdQuestion,
            vote.TipoVoto,
            vote.Fecha
        ));
    }
    [HttpGet("referendums/{idReferendum:int}/summary")]
    public async Task<ActionResult<ReferendumSummaryResponse>> GetSummaryAsync(
        int idReferendum,
        CancellationToken ct)
    {
        var votes = await _context.Votes
            .AsNoTracking()
            .Where(v => v.IdReferendum == idReferendum)
            .ToListAsync(ct);

        var summary = votes
            .GroupBy(v => v.IdQuestion)
            .Select(group => new QuestionSummaryResponse(
                group.Key,
                group.Count(v => v.TipoVoto == "SI"),
                group.Count(v => v.TipoVoto == "NO"),
                group.Count(v => v.TipoVoto == "BLANCO"),
                group.Count(v => v.TipoVoto == "NULO"),
                group.Count()
            ))
            .OrderBy(q => q.IdQuestion)
            .ToList();

        return Ok(new ReferendumSummaryResponse(
            idReferendum,
            summary
        ));
    }

    [HttpPost]
    public async Task<ActionResult<VoteResponse>> CreateAsync(
        CreateVoteRequest request,
        CancellationToken ct)
    {
        var tipoVoto = request.TipoVoto.Trim().ToUpperInvariant();

        if (!TiposPermitidos.Contains(tipoVoto))
        {
            return BadRequest(new
            {
                error = "Tipo de voto inválido. Valores permitidos: SI, NO, BLANCO, NULO"
            });
        }
        var eligibility = await _referendumClient.CheckEligibilityAsync(
            request.IdReferendum,
            request.IdQuestion,
            request.IdVotante,
            ct);

        if (eligibility is null)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { error = "No se pudo consultar la elegibilidad del votante" });
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

        var marked = await _referendumClient.MarkVotedAsync(
    request.IdReferendum,
    request.IdQuestion,
    request.IdVotante,
    ct);

        if (!marked)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error = "El voto fue registrado, pero no se pudo marcar al votante como que ya votó"
                });
        }

        var response = new VoteResponse(
            vote.IdVote,
            vote.IdReferendum,
            vote.IdQuestion,
            vote.TipoVoto,
            vote.Fecha
        );

        return Ok(response);
    }

    [HttpGet("referendums/{idReferendum:int}")]
    public async Task<ActionResult<IEnumerable<VoteResponse>>> GetByReferendumAsync(
        int idReferendum,
        CancellationToken ct)
    {
        var votes = await _context.Votes
            .AsNoTracking()
            .Where(v => v.IdReferendum == idReferendum)
            .ToListAsync(ct);

        return Ok(votes.Select(v => new VoteResponse(
            v.IdVote,
            v.IdReferendum,
            v.IdQuestion,
            v.TipoVoto,
            v.Fecha
        )));
    }
}