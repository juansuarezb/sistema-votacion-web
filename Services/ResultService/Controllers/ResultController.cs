using Microsoft.AspNetCore.Mvc;
using ResultService.Models;
using ResultService.Services;

namespace ResultService.Controllers;


// Expone las operaciones HTTP utilizadas para consultar resultados
[ApiController]
[Route("api/[controller]")]
public sealed class ResultsController : ControllerBase
{
    private readonly VoteClient _voteClient;
    private readonly ILogger<ResultsController> _logger;

  
    // Inicializa una nueva instancia de "ResultsController"

    public ResultsController(
        VoteClient voteClient,
        ILogger<ResultsController> logger)
    {
        ArgumentNullException.ThrowIfNull(voteClient);
        ArgumentNullException.ThrowIfNull(logger);

        _voteClient = voteClient;
        _logger = logger;
    }

 
    // Obtiene los resultados agregados de un referéndum y calcula los
    //porcentajes correspondientes a cada tipo de voto.

    [HttpGet("referendums/{idReferendum:int}")]
    [ProducesResponseType(
        typeof(ReferendumResultResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ReferendumResultResponse>>
        GetResultsByReferendumAsync(
            int idReferendum,
            CancellationToken ct)
    {
        if (idReferendum <= 0)
        {
            return BadRequest(new
            {
                error = "El identificador del referéndum no es válido."
            });
        }

        try
        {
            var summary =
                await _voteClient.GetReferendumSummaryAsync(
                    idReferendum,
                    ct
                );

            if (summary is null)
            {
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new
                    {
                        error =
                            "VoteService devolvió una respuesta vacía."
                    }
                );
            }

            var questions = summary.Questions
                .Select(ToQuestionResult)
                .OrderBy(question => question.IdQuestion)
                .ToList();

            /*
             * TotalVotes representa la suma de respuestas registradas en
             * todas las preguntas. No equivale necesariamente al número de
             * votantes si el referéndum contiene varias preguntas.
             */
            var totalVotes =
                questions.Sum(question => question.Total);

            return Ok(new ReferendumResultResponse(
                summary.IdReferendum,
                totalVotes,
                questions
            ));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "No se pudo consultar VoteService para el referéndum {IdReferendum}.",
                idReferendum
            );

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    error =
                        "No se pudo establecer comunicación con VoteService."
                }
            );
        }
        catch (TaskCanceledException ex)
            when (!ct.IsCancellationRequested)
        {
            _logger.LogError(
                ex,
                "VoteService excedió el tiempo de espera para el referéndum {IdReferendum}.",
                idReferendum
            );

            return StatusCode(
                StatusCodes.Status504GatewayTimeout,
                new
                {
                    error =
                        "VoteService excedió el tiempo máximo de respuesta."
                }
            );
        }
    }

  
    // Convierte el resumen de una pregunta devuelto por VoteService en el
    // resultado expuesto por ResultService.
    private static QuestionResultResponse ToQuestionResult(
        QuestionSummaryFromVoteService summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        return new QuestionResultResponse(
            summary.IdQuestion,
            summary.Si,
            summary.No,
            summary.Blanco,
            summary.Nulo,
            summary.Total,
            CalculatePercentage(summary.Si, summary.Total),
            CalculatePercentage(summary.No, summary.Total),
            CalculatePercentage(summary.Blanco, summary.Total),
            CalculatePercentage(summary.Nulo, summary.Total)
        );
    }

   
    // Calcula el porcentaje que representa una cantidad dentro de un total.
    private static decimal CalculatePercentage(
        int value,
        int total)
    {
        if (total <= 0)
        {
            return 0;
        }

        return Math.Round(
            (decimal)value * 100 / total,
            2,
            MidpointRounding.AwayFromZero
        );
    }
}