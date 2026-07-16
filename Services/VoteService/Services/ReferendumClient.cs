using System.Net.Http.Json;
using VoteService.Models;

namespace VoteService.Services;

/// <summary>
/// Encapsula la comunicación HTTP de VoteService con ReferendumService para
/// validar elegibilidad y actualizar el estado de las asignaciones.
/// </summary>
public sealed class ReferendumClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ReferendumClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// Cliente HTTP configurado con la dirección base de ReferendumService.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando <paramref name="httpClient"/> es
    /// <see langword="null"/>.
    /// </exception>
    public ReferendumClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    /// <summary>
    /// Consulta si un votante puede responder una pregunta determinada.
    /// </summary>
    /// <param name="idReferendum">Identificador del referéndum.</param>
    /// <param name="idQuestion">Identificador de la pregunta.</param>
    /// <param name="idVotante">Identificador interno del votante.</param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// La información de elegibilidad devuelta por ReferendumService o
    /// <see langword="null"/> cuando la respuesta no contiene un objeto.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando ReferendumService devuelve una respuesta HTTP no
    /// satisfactoria o falla la comunicación.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    public async Task<EligibilityResponse?> CheckEligibilityAsync(
        int idReferendum,
        int idQuestion,
        int idVotante,
        CancellationToken ct)
    {
        return await _httpClient.GetFromJsonAsync<EligibilityResponse>(
            $"/api/referendums/{idReferendum}/questions/" +
            $"{idQuestion}/voters/{idVotante}/eligibility",
            ct
        );
    }

    /// <summary>
    /// Marca una asignación como respondida en ReferendumService.
    /// </summary>
    /// <param name="idReferendum">Identificador del referéndum.</param>
    /// <param name="idQuestion">Identificador de la pregunta.</param>
    /// <param name="idVotante">Identificador interno del votante.</param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// <see langword="true"/> cuando ReferendumService acepta la actualización;
    /// de lo contrario, <see langword="false"/>.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando falla la comunicación HTTP.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    public async Task<bool> MarkVotedAsync(
        int idReferendum,
        int idQuestion,
        int idVotante,
        CancellationToken ct)
    {
        using var response = await _httpClient.PatchAsync(
            $"/api/referendums/{idReferendum}/questions/" +
            $"{idQuestion}/voters/{idVotante}/mark-voted",
            content: null,
            ct
        );

        return response.IsSuccessStatusCode;
    }
}