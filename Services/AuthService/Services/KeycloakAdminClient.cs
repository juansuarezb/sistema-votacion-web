using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AuthService.Options;
using Microsoft.Extensions.Options;

namespace AuthService.Services;

/// <summary>
/// Encapsula las operaciones administrativas ejecutadas contra la API REST
/// de Keycloak mediante las credenciales de un cliente técnico.
/// </summary>
/// <remarks>
/// El cliente obtiene tokens mediante el flujo Client Credentials y utiliza
/// dichos tokens para crear usuarios, asignar roles de realm y eliminar
/// identidades.
/// </remarks>
public sealed class KeycloakAdminClient
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _options;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="KeycloakAdminClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// Cliente HTTP administrado por <see cref="IHttpClientFactory"/>.
    /// </param>
    /// <param name="options">
    /// Configuración requerida para conectarse al realm y al cliente técnico
    /// de Keycloak.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce si <paramref name="httpClient"/> o
    /// <paramref name="options"/> es <see langword="null"/>.
    /// </exception>
    public KeycloakAdminClient(
        HttpClient httpClient,
        IOptions<KeycloakOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <summary>
    /// Crea una identidad en Keycloak, configura su contraseña permanente y
    /// le asigna un rol de realm.
    /// </summary>
    /// <param name="username">
    /// Nombre de usuario único con el que la persona iniciará sesión.
    /// </param>
    /// <param name="nombre">
    /// Nombre completo utilizado para construir los atributos
    /// <c>firstName</c> y <c>lastName</c> en Keycloak.
    /// </param>
    /// <param name="email">
    /// Dirección de correo electrónico asociada a la identidad.
    /// </param>
    /// <param name="password">
    /// Contraseña permanente inicial del usuario.
    /// </param>
    /// <param name="roleName">
    /// Nombre del rol de realm que debe asignarse al usuario.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// El identificador UUID generado por Keycloak para la nueva identidad.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Se produce si el usuario ya existe, Keycloak rechaza la creación,
    /// no devuelve el identificador generado o no permite asignar el rol.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando ocurre un error de comunicación con Keycloak.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante <paramref name="ct"/>.
    /// </exception>
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
            .Split(
                ' ',
                2,
                StringSplitOptions.RemoveEmptyEntries
            );

        // El primer espacio separa nombres y apellidos sin imponer una
        // estructura cultural rígida al nombre completo.
        var firstName = nameParts[0];
        var lastName =
            nameParts.Length > 1
                ? nameParts[1]
                : string.Empty;

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

        using var response =
            await _httpClient.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new InvalidOperationException(
                "Ya existe un usuario con ese username o correo en Keycloak."
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            var content =
                await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"Keycloak rechazó la creación del usuario: " +
                $"{(int)response.StatusCode} {content}"
            );
        }

        var location =
            response.Headers.Location?.ToString();

        if (string.IsNullOrWhiteSpace(location))
        {
            throw new InvalidOperationException(
                "Keycloak creó el usuario, pero no devolvió su identificador."
            );
        }

        // Keycloak devuelve el UUID creado en el último segmento del header
        // Location de la respuesta HTTP 201.
        var userId =
            location.TrimEnd('/').Split('/').Last();

        await AssignRealmRoleAsync(
            userId,
            roleName,
            token,
            ct
        );

        return userId;
    }

    /// <summary>
    /// Elimina una identidad de Keycloak utilizando su identificador UUID.
    /// </summary>
    /// <param name="userId">
    /// Identificador UUID de la identidad que debe eliminarse.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Una tarea que representa la eliminación. La operación se considera
    /// correcta si el usuario se elimina o si Keycloak informa que ya no existe.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Se produce si Keycloak devuelve un estado distinto de éxito o 404.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando ocurre un error de comunicación con Keycloak.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante <paramref name="ct"/>.
    /// </exception>
    public async Task DeleteUserAsync(
        string userId,
        CancellationToken ct)
    {
        var token = await GetServiceTokenAsync(ct);

        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"{_options.BaseUrl}/admin/realms/" +
            $"{_options.Realm}/users/{userId}"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response =
            await _httpClient.SendAsync(request, ct);

        // Un 404 produce el mismo estado final deseado: la identidad ya no
        // está presente en Keycloak.
        if (response.StatusCode != HttpStatusCode.NotFound &&
            !response.IsSuccessStatusCode)
        {
            var content =
                await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se pudo eliminar el usuario en Keycloak: " +
                $"{(int)response.StatusCode} {content}"
            );
        }
    }

    /// <summary>
    /// Asigna un rol de realm existente a una identidad de Keycloak.
    /// </summary>
    /// <param name="userId">
    /// Identificador UUID del usuario que recibirá el rol.
    /// </param>
    /// <param name="roleName">
    /// Nombre del rol de realm que debe asignarse.
    /// </param>
    /// <param name="token">
    /// Token administrativo utilizado para autorizar las llamadas REST.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Una tarea que representa la consulta y asignación del rol.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Se produce si el rol no existe, no puede consultarse o Keycloak rechaza
    /// su asignación.
    /// </exception>
    /// <exception cref="JsonException">
    /// Se produce si Keycloak devuelve una representación de rol inválida.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando ocurre un error de comunicación con Keycloak.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante <paramref name="ct"/>.
    /// </exception>
    private async Task AssignRealmRoleAsync(
        string userId,
        string roleName,
        string token,
        CancellationToken ct)
    {
        using var roleRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"{_options.BaseUrl}/admin/realms/" +
            $"{_options.Realm}/roles/{roleName}"
        );

        roleRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var roleResponse =
            await _httpClient.SendAsync(roleRequest, ct);

        if (!roleResponse.IsSuccessStatusCode)
        {
            var content =
                await roleResponse.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se encontró o no se pudo leer el rol " +
                $"{roleName}: {content}"
            );
        }

        var role =
            await roleResponse.Content.ReadFromJsonAsync<JsonElement>(
                cancellationToken: ct
            );

        using var assignRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.BaseUrl}/admin/realms/" +
            $"{_options.Realm}/users/{userId}/role-mappings/realm"
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

        using var assignResponse =
            await _httpClient.SendAsync(assignRequest, ct);

        if (!assignResponse.IsSuccessStatusCode)
        {
            var content =
                await assignResponse.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"No se pudo asignar el rol {roleName}: {content}"
            );
        }

        // FIXME: Si la creación funciona pero la asignación del rol falla,
        // la identidad queda creada sin permisos. Considerar eliminarla como
        // compensación dentro de este cliente.
    }

    /// <summary>
    /// Obtiene un token administrativo mediante el flujo OAuth 2.0
    /// Client Credentials.
    /// </summary>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// El access token emitido para la cuenta de servicio configurada.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Se produce si Keycloak rechaza las credenciales o no devuelve un token.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Se produce cuando ocurre un error de comunicación con Keycloak.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante <paramref name="ct"/>.
    /// </exception>
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
            $"{_options.BaseUrl}/realms/{_options.Realm}/" +
            "protocol/openid-connect/token",
            content,
            ct
        );

        if (!response.IsSuccessStatusCode)
        {
            var responseBody =
                await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                "No se pudo obtener token administrativo de Keycloak: " +
                responseBody
            );
        }

        var tokenResponse =
            await response.Content.ReadFromJsonAsync<ServiceTokenResponse>(
                cancellationToken: ct
            );

        return tokenResponse?.AccessToken
            ?? throw new InvalidOperationException(
                "Keycloak no devolvió un access token."
            );
    }

    /// <summary>
    /// Representa la parte de la respuesta OAuth necesaria para obtener el
    /// access token del cliente técnico.
    /// </summary>
    private sealed class ServiceTokenResponse
    {
        /// <summary>
        /// Obtiene o establece el access token emitido por Keycloak.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}