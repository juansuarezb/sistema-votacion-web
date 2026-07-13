using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AuthService.Options;
using Microsoft.Extensions.Options;

namespace AuthService.Services;

public sealed class KeycloakAdminClient
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _options;

    public KeycloakAdminClient(
        HttpClient httpClient,
        IOptions<KeycloakOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> CreateUserAsync(
        string username,
        string nombre,
        string email,
        string password,
        string roleName,
        CancellationToken ct)
    {
        var token = await GetServiceTokenAsync(ct);

        var nameParts = nombre
            .Trim()
            .Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(new
        {
            username = username.Trim(),
            email = email.Trim(),
            firstName,
            lastName,
            enabled = true,
            emailVerified = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = password,
                    temporary = false
                }
            }
        });

        using var response = await _httpClient.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new InvalidOperationException(
                "Ya existe un usuario con ese username o correo en Keycloak."
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"Keycloak rechazó la creación del usuario: {(int)response.StatusCode} {content}"
            );
        }

        var location = response.Headers.Location?.ToString();

        if (string.IsNullOrWhiteSpace(location))
        {
            throw new InvalidOperationException(
                "Keycloak creó el usuario, pero no devolvió su identificador."
            );
        }

        var userId = location.TrimEnd('/').Split('/').Last();

        await AssignRealmRoleAsync(userId, roleName, token, ct);

        return userId;
    }

    public async Task DeleteUserAsync(
        string userId,
        CancellationToken ct)
    {
        var token = await GetServiceTokenAsync(ct);

        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response = await _httpClient.SendAsync(request, ct);

        if (response.StatusCode != HttpStatusCode.NotFound &&
            !response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se pudo eliminar el usuario compensatorio en Keycloak: {content}"
            );
        }
    }

    private async Task AssignRealmRoleAsync(
        string userId,
        string roleName,
        string token,
        CancellationToken ct)
    {
        using var roleRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_options.BaseUrl}/admin/realms/{_options.Realm}/roles/{roleName}"
        );

        roleRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var roleResponse = await _httpClient.SendAsync(roleRequest, ct);

        if (!roleResponse.IsSuccessStatusCode)
        {
            var content = await roleResponse.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se encontró o no se pudo leer el rol {roleName}: {content}"
            );
        }

        var role = await roleResponse.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken: ct
        );

        using var assignRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm"
        );

        assignRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        assignRequest.Content = JsonContent.Create(new[]
        {
            new
            {
                id = role.GetProperty("id").GetString(),
                name = role.GetProperty("name").GetString()
            }
        });

        using var assignResponse = await _httpClient.SendAsync(assignRequest, ct);

        if (!assignResponse.IsSuccessStatusCode)
        {
            var content = await assignResponse.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se pudo asignar el rol {roleName}: {content}"
            );
        }
    }

    private async Task<string> GetServiceTokenAsync(
        CancellationToken ct)
    {
        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret
            }
        );

        using var response = await _httpClient.PostAsync(
            $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token",
            content,
            ct
        );

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se pudo obtener token administrativo de Keycloak: {responseBody}"
            );
        }

        var tokenResponse = await response.Content
            .ReadFromJsonAsync<ServiceTokenResponse>(cancellationToken: ct);

        return tokenResponse?.AccessToken
            ?? throw new InvalidOperationException(
                "Keycloak no devolvió un access token."
            );
    }

    private sealed class ServiceTokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}