using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AuthService.Models;
using AuthService.Options;
using Microsoft.Extensions.Options;

namespace AuthService.Services;

public sealed class VoterServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ServiceOptions _options;

    public VoterServiceClient(
        HttpClient httpClient,
        IOptions<ServiceOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<JsonElement> CreateProfileAsync(
        CreateVoterProfileRequest request,
        CancellationToken ct)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            $"{_options.VoterServiceUrl}/api/votantes",
            request,
            ct
        );

        var content = await response.Content.ReadAsStringAsync(ct);

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

        return JsonSerializer.Deserialize<JsonElement>(content);
    }

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
            var content = await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se pudo consultar el votante en VoterService: " +
                $"{(int)response.StatusCode} {content}"
            );
        }

        var voter = await response.Content.ReadFromJsonAsync<VoterProfileResponse>(
            cancellationToken: ct
        );

        return voter ?? throw new InvalidOperationException(
            "VoterService devolvió una respuesta vacía."
        );
    }

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
            var content = await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se pudo eliminar el perfil en VoterService: " +
                $"{(int)response.StatusCode} {content}"
            );
        }
    }
}

public sealed record VoterProfileResponse(
    int IdVotante,
    string KeycloakId,
    string Nombre,
    string Cedula,
    string CorreoElectronico,
    DateTime FechaRegistro
);