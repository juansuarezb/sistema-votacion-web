using Microsoft.AspNetCore.Mvc;
using ResultService.Models;
using ResultService.Services;

namespace ResultService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly VoteClient _voteClient;

    public ResultsController(VoteClient voteClient)
    {
        _voteClient = voteClient;
    }

    [HttpGet("referendums/{idReferendum:int}")]
    public async Task<ActionResult<ReferendumResultResponse>> GetResultsByReferendumAsync(
        int idReferendum,
        CancellationToken ct)
    {
        var summary = await _voteClient.GetReferendumSummaryAsync(idReferendum, ct);

        if (summary is null)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { error = "No se pudo obtener el resumen de votos desde VoteService" });
        }

        var questions = summary.Questions.Select(q =>
        {
            decimal Percent(int value)
            {
                if (q.Total == 0)
                    return 0;

                return Math.Round((decimal)value * 100 / q.Total, 2);
            }

            return new QuestionResultResponse(
                q.IdQuestion,
                q.Si,
                q.No,
                q.Blanco,
                q.Nulo,
                q.Total,
                Percent(q.Si),
                Percent(q.No),
                Percent(q.Blanco),
                Percent(q.Nulo)
            );
        }).ToList();

        var totalVotes = questions.Sum(q => q.Total);

        return Ok(new ReferendumResultResponse(
            summary.IdReferendum,
            totalVotes,
            questions
        ));
    }
}