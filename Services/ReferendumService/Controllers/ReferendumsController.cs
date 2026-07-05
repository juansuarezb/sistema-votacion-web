using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReferendumService.Data;
using ReferendumService.Models;

namespace ReferendumService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReferendumsController : ControllerBase
{
    private readonly ReferendumDbContext _context;

    public ReferendumsController(ReferendumDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReferendumResponse>>> GetAllAsync(CancellationToken ct)
    {
        var referendums = await _context.Referendums
            .AsNoTracking()
            .ToListAsync(ct);

        return Ok(referendums.Select(r => new ReferendumResponse(
            r.IdReferendum,
            r.Titulo,
            r.Descripcion,
            r.FechaInicio,
            r.FechaCierre,
            r.Estado,
            r.FechaCreacion
        )));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReferendumResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var r = await _context.Referendums
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdReferendum == id, ct);

        if (r is null)
            return NotFound(new { mensaje = "Referéndum no encontrado" });

        return Ok(new ReferendumResponse(
            r.IdReferendum,
            r.Titulo,
            r.Descripcion,
            r.FechaInicio,
            r.FechaCierre,
            r.Estado,
            r.FechaCreacion
        ));
    }

    [HttpPost]
    public async Task<ActionResult<ReferendumResponse>> CreateAsync(
        CreateReferendumRequest request,
        CancellationToken ct)
    {
        if (request.FechaCierre <= request.FechaInicio)
            return BadRequest(new { error = "La fecha de cierre debe ser mayor a la fecha de inicio" });

        var referendum = new Referendum
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            FechaInicio = request.FechaInicio,
            FechaCierre = request.FechaCierre,
            Estado = "BORRADOR"
        };

        _context.Referendums.Add(referendum);
        await _context.SaveChangesAsync(ct);

        return Ok(new ReferendumResponse(
            referendum.IdReferendum,
            referendum.Titulo,
            referendum.Descripcion,
            referendum.FechaInicio,
            referendum.FechaCierre,
            referendum.Estado,
            referendum.FechaCreacion
        ));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(
        int id,
        UpdateReferendumRequest request,
        CancellationToken ct)
    {
        var referendum = await _context.Referendums.FindAsync(new object[] { id }, ct);

        if (referendum is null)
            return NotFound(new { mensaje = "Referéndum no encontrado" });

        if (request.FechaCierre <= request.FechaInicio)
            return BadRequest(new { error = "La fecha de cierre debe ser mayor a la fecha de inicio" });

        referendum.Titulo = request.Titulo;
        referendum.Descripcion = request.Descripcion;
        referendum.FechaInicio = request.FechaInicio;
        referendum.FechaCierre = request.FechaCierre;
        referendum.Estado = request.Estado;

        await _context.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct)
    {
        var referendum = await _context.Referendums.FindAsync(new object[] { id }, ct);

        if (referendum is null)
            return NotFound(new { mensaje = "Referéndum no encontrado" });

        _context.Referendums.Remove(referendum);
        await _context.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpPost("{id:int}/questions")]
    public async Task<ActionResult<QuestionResponse>> AddQuestionAsync(
        int id,
        CreateQuestionRequest request,
        CancellationToken ct)
    {
        var exists = await _context.Referendums.AnyAsync(r => r.IdReferendum == id, ct);

        if (!exists)
            return NotFound(new { mensaje = "Referéndum no encontrado" });

        var question = new ReferendumQuestion
        {
            IdReferendum = id,
            Texto = request.Texto
        };

        _context.ReferendumQuestions.Add(question);
        await _context.SaveChangesAsync(ct);

        return Ok(new QuestionResponse(
            question.IdQuestion,
            question.IdReferendum,
            question.Texto
        ));
    }

    [HttpGet("{id:int}/questions")]
    public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetQuestionsAsync(
        int id,
        CancellationToken ct)
    {
        var questions = await _context.ReferendumQuestions
            .AsNoTracking()
            .Where(q => q.IdReferendum == id)
            .ToListAsync(ct);

        return Ok(questions.Select(q => new QuestionResponse(
            q.IdQuestion,
            q.IdReferendum,
            q.Texto
        )));
    }

    [HttpPost("{id:int}/voters")]
    public async Task<IActionResult> AssignVoterAsync(
        int id,
        AssignVoterRequest request,
        CancellationToken ct)
    {
        var referendum = await _context.Referendums
            .Include(r => r.Preguntas)
            .FirstOrDefaultAsync(r => r.IdReferendum == id, ct);

        if (referendum is null)
            return NotFound(new { mensaje = "Referéndum no encontrado" });

        if (!referendum.Preguntas.Any())
            return BadRequest(new { error = "El referéndum no tiene preguntas registradas" });

        foreach (var question in referendum.Preguntas)
        {
            var exists = await _context.ReferendumQuestionVoters.AnyAsync(
                x => x.IdReferendum == id &&
                     x.IdQuestion == question.IdQuestion &&
                     x.IdVotante == request.IdVotante,
                ct);

            if (!exists)
            {
                _context.ReferendumQuestionVoters.Add(new ReferendumQuestionVoter
                {
                    IdReferendum = id,
                    IdQuestion = question.IdQuestion,
                    IdVotante = request.IdVotante,
                    HaVotado = false
                });
            }
        }

        await _context.SaveChangesAsync(ct);

        return Ok(new { mensaje = "Votante asignado a todas las preguntas del referéndum" });
    }

    [HttpGet("{id:int}/voters")]
    public async Task<IActionResult> GetAssignedVotersAsync(int id, CancellationToken ct)
    {
        var voters = await _context.ReferendumQuestionVoters
            .AsNoTracking()
            .Where(x => x.IdReferendum == id)
            .OrderBy(x => x.IdVotante)
            .ThenBy(x => x.IdQuestion)
            .ToListAsync(ct);

        return Ok(voters);
    }

    [HttpGet("{id:int}/questions/{idQuestion:int}/voters/{idVotante:int}/eligibility")]
    public async Task<ActionResult<EligibilityResponse>> CheckEligibilityAsync(
        int id,
        int idQuestion,
        int idVotante,
        CancellationToken ct)
    {
        var referendum = await _context.Referendums
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.IdReferendum == id, ct);

        if (referendum is null)
            return NotFound(new { mensaje = "Referéndum no encontrado" });

        var questionExists = await _context.ReferendumQuestions
            .AnyAsync(q => q.IdReferendum == id && q.IdQuestion == idQuestion, ct);

        if (!questionExists)
            return NotFound(new { mensaje = "Pregunta no encontrada para este referéndum" });

        var assignment = await _context.ReferendumQuestionVoters
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.IdReferendum == id &&
                x.IdQuestion == idQuestion &&
                x.IdVotante == idVotante,
                ct);

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

        if (now < referendum.FechaInicio || now > referendum.FechaCierre)
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

    [HttpPatch("{id:int}/questions/{idQuestion:int}/voters/{idVotante:int}/mark-voted")]
    public async Task<IActionResult> MarkVotedAsync(
        int id,
        int idQuestion,
        int idVotante,
        CancellationToken ct)
    {
        var assignment = await _context.ReferendumQuestionVoters
            .FirstOrDefaultAsync(x =>
                x.IdReferendum == id &&
                x.IdQuestion == idQuestion &&
                x.IdVotante == idVotante,
                ct);

        if (assignment is null)
            return NotFound(new { mensaje = "Asignación no encontrada" });

        if (assignment.HaVotado)
            return Conflict(new { error = "El votante ya respondió esta pregunta" });

        assignment.HaVotado = true;
        assignment.FechaVoto = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return NoContent();
    }
}