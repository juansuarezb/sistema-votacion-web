using System.Net;
using System.Net.Http.Json;
using ResultService.Models;

namespace ResultService.Services;

/// <summary>
/// Encapsula la comunicación HTTP entre ResultService y VoteService.
/// </summary>
public sealed class VoteClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VoteClient> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VoteClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// Cliente HTTP configurado con la dirección base de VoteService.
    /// </param>
    /// <param name="logger">
    /// Servicio utilizado para registrar errores de comunicación.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando alguna dependencia es
    /// <see langword="null"/>.
    /// </exception>
    public VoteClient(
        HttpClient httpClient,
        ILogger<VoteClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(logger);

        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene desde VoteService el resumen agregado de votos de un
    /// referéndum.
    /// </summary>
    /// <param name="idReferendum">
    /// Identificador del referéndum que debe consultarse.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// El resumen proporcionado por VoteService, o
    /// <see langword="null"/> cuando no existe información.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Se produce cuando <paramref name="idReferendum"/> no es positivo.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando VoteService devuelve una respuesta no satisfactoria
    /// o falla la comunicación.
    /// </exception>
    public async Task<ReferendumSummaryFromVoteService?>
        GetReferendumSummaryAsync(
            int idReferendum,
            CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
            idReferendum
        );

        var requestUri =
            $"/api/votes/referendums/{idReferendum}/summary";

        using var response =
            await _httpClient.GetAsync(requestUri, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var content =
                await response.Content.ReadAsStringAsync(ct);

            _logger.LogWarning(
                "VoteService devolvió HTTP {StatusCode} al consultar el referéndum {IdReferendum}. Respuesta: {ResponseBody}",
                (int)response.StatusCode,
                idReferendum,
                content
            );

            throw new HttpRequestException(
                $"VoteService devolvió HTTP {(int)response.StatusCode}.",
                inner: null,
                response.StatusCode
            );
        }

        return await response.Content
            .ReadFromJsonAsync<ReferendumSummaryFromVoteService>(
                cancellationToken: ct
            );
    }
}