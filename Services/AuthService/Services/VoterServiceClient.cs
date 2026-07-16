using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AuthService.Models;
using AuthService.Options;
using Microsoft.Extensions.Options;

namespace AuthService.Services;

/// <summary>
/// Encapsula la comunicación HTTP entre AuthService y VoterService para
/// administrar perfiles electorales.
/// </summary>
public sealed class VoterServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ServiceOptions _options;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VoterServiceClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// Cliente HTTP administrado por <see cref="IHttpClientFactory"/>.
    /// </param>
    /// <param name="options">
    /// Configuración que contiene la dirección interna de VoterService.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce si <paramref name="httpClient"/> o
    /// <paramref name="options"/> es <see langword="null"/>.
    /// </exception>
    public VoterServiceClient(
        HttpClient httpClient,
        IOptions<ServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <summary>
    /// Crea en VoterService el perfil electoral asociado a una identidad de
    /// Keycloak.
    /// </summary>
    /// <param name="request">
    /// Información necesaria para crear el perfil electoral.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// La representación JSON del perfil electoral creado.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Se produce si ya existe un perfil con la misma cédula o correo, si
    /// VoterService rechaza la solicitud o si devuelve contenido JSON inválido.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando ocurre un error de comunicación con VoterService.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante <paramref name="ct"/>.
    /// </exception>
    public async Task<JsonElement> CreateProfileAsync(
        CreateVoterProfileRequest request,
        CancellationToken ct)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            $"{_options.VoterServiceUrl}/api/votantes",
            request,
            ct
        );

        var content =
            await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new InvalidOperationException(
                "Ya existe un votante con esa cédula o correo."
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"VoterService rechazó el perfil: " +
                $"{(int)response.StatusCode} {content}"
            );
        }

        try
        {
            return JsonSerializer.Deserialize<JsonElement>(content);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "VoterService devolvió un perfil con formato JSON inválido.",
                ex
            );
        }
    }

    /// <summary>
    /// Obtiene un perfil electoral mediante su identificador interno.
    /// </summary>
    /// <param name="idVotante">
    /// Identificador interno del perfil almacenado en VoterService.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// El perfil electoral correspondiente al identificador proporcionado.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Se produce si el perfil solicitado no existe.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Se produce si VoterService rechaza la consulta o devuelve una respuesta
    /// vacía o inválida.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando ocurre un error de comunicación con VoterService.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante <paramref name="ct"/>.
    /// </exception>
    public async Task<VoterProfileResponse> GetProfileByIdAsync(
        int idVotante,
        CancellationToken ct)
    {
        using var response = await _httpClient.GetAsync(
            $"{_options.VoterServiceUrl}/api/votantes/{idVotante}",
            ct
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new KeyNotFoundException(
                "El votante no existe en VoterService."
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            var content =
                await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                "No se pudo consultar el votante en VoterService: " +
                $"{(int)response.StatusCode} {content}"
            );
        }

        var voter =
            await response.Content
                .ReadFromJsonAsync<VoterProfileResponse>(
                    cancellationToken: ct
                );

        return voter
            ?? throw new InvalidOperationException(
                "VoterService devolvió una respuesta vacía."
            );
    }

    /// <summary>
    /// Elimina un perfil electoral de VoterService.
    /// </summary>
    /// <param name="idVotante">
    /// Identificador interno del perfil que debe eliminarse.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Una tarea que representa la eliminación del perfil.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Se produce si el perfil solicitado no existe.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Se produce si VoterService rechaza la eliminación.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando ocurre un error de comunicación con VoterService.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante <paramref name="ct"/>.
    /// </exception>
    public async Task DeleteProfileAsync(
        int idVotante,
        CancellationToken ct)
    {
        using var response = await _httpClient.DeleteAsync(
            $"{_options.VoterServiceUrl}/api/votantes/{idVotante}",
            ct
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new KeyNotFoundException(
                "El perfil del votante no existe en VoterService."
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            var content =
                await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                "No se pudo eliminar el perfil en VoterService: " +
                $"{(int)response.StatusCode} {content}"
            );
        }
    }
}

/// <summary>
/// Representa la información de un perfil electoral obtenida desde
/// VoterService.
/// </summary>
/// <param name="IdVotante">
/// Identificador interno del perfil electoral.
/// </param>
/// <param name="KeycloakId">
/// Identificador UUID de la identidad correspondiente en Keycloak.
/// </param>
/// <param name="Nombre">
/// Nombre completo del votante.
/// </param>
/// <param name="Cedula">
/// Número de identificación del votante.
/// </param>
/// <param name="CorreoElectronico">
/// Dirección de correo electrónico del votante.
/// </param>
/// <param name="FechaRegistro">
/// Fecha y hora en que se registró el perfil electoral.
/// </param>
public sealed record VoterProfileResponse(
    int IdVotante,
    string KeycloakId,
    string Nombre,
    string Cedula,
    string CorreoElectronico,
    DateTime FechaRegistro
);