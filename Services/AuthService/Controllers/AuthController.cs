using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

/// <summary>
/// Expone las operaciones HTTP relacionadas con el registro y la eliminación
/// de identidades administradas mediante Keycloak y VoterService.
/// </summary>
/// <remarks>
/// Este controlador coordina operaciones distribuidas entre Keycloak y
/// VoterService. Cuando la creación del perfil electoral falla después de
/// crear la identidad, ejecuta una eliminación compensatoria en Keycloak.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly KeycloakAdminClient _keycloakAdminClient;
    private readonly VoterServiceClient _voterServiceClient;
    private readonly AuditClient _auditClient;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AuthController"/>.
    /// </summary>
    /// <param name="keycloakAdminClient">
    /// Cliente utilizado para administrar identidades en Keycloak.
    /// </param>
    /// <param name="voterServiceClient">
    /// Cliente utilizado para administrar perfiles electorales.
    /// </param>
    /// <param name="auditClient">
    /// Cliente utilizado para registrar eventos funcionales.
    /// </param>
    /// <param name="logger">
    /// Servicio utilizado para registrar errores internos.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando una dependencia es <see langword="null"/>.
    /// </exception>
    public AuthController(
        KeycloakAdminClient keycloakAdminClient,
        VoterServiceClient voterServiceClient,
        AuditClient auditClient,
        ILogger<AuthController> logger)
    {
        ArgumentNullException.ThrowIfNull(keycloakAdminClient);
        ArgumentNullException.ThrowIfNull(voterServiceClient);
        ArgumentNullException.ThrowIfNull(auditClient);
        ArgumentNullException.ThrowIfNull(logger);

        _keycloakAdminClient = keycloakAdminClient;
        _voterServiceClient = voterServiceClient;
        _auditClient = auditClient;
        _logger = logger;
    }

    /// <summary>
    /// Registra una nueva identidad administrativa en Keycloak y le asigna
    /// el rol <c>ADMIN</c>.
    /// </summary>
    /// <param name="request">
    /// Datos requeridos para crear la cuenta administrativa.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación.
    /// </param>
    /// <returns>
    /// HTTP 201 cuando el administrador se registra, 400 cuando Keycloak
    /// rechaza los datos o 503 cuando no puede establecerse comunicación.
    /// </returns>
    [HttpPost("register-admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> RegisterAdminAsync(
        RegisterAdminRequest request,
        CancellationToken ct)
    {
        try
        {
            var keycloakUserId =
                await _keycloakAdminClient.CreateUserAsync(
                    request.Username,
                    request.Nombre,
                    request.CorreoElectronico,
                    request.Password,
                    "ADMIN",
                    ct
                );

            await WriteAuditEventAsync(
                action: "ADMIN_CREATED",
                entityType: "KeycloakUser",
                entityId: keycloakUserId,
                username: request.Username.Trim(),
                httpMethod: HttpMethods.Post,
                path: "/api/auth/register-admin",
                statusCode: StatusCodes.Status201Created,
                success: true,
                description:
                    $"Se registró la cuenta administrativa '{request.Username.Trim()}'.",
                ct
            );

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = "Administrador registrado correctamente.",
                    keycloakId = keycloakUserId
                }
            );
        }
        catch (InvalidOperationException ex)
        {
            await WriteAuditEventAsync(
                action: "ADMIN_REGISTRATION_FAILED",
                entityType: "KeycloakUser",
                entityId: null,
                username: request.Username.Trim(),
                httpMethod: HttpMethods.Post,
                path: "/api/auth/register-admin",
                statusCode: StatusCodes.Status400BadRequest,
                success: false,
                description: ex.Message,
                CancellationToken.None
            );

            return BadRequest(new
            {
                error = ex.Message
            });
        }
        catch (HttpRequestException ex)
        {
            await WriteAuditEventAsync(
                action: "ADMIN_REGISTRATION_FAILED",
                entityType: "KeycloakUser",
                entityId: null,
                username: request.Username.Trim(),
                httpMethod: HttpMethods.Post,
                path: "/api/auth/register-admin",
                statusCode: StatusCodes.Status503ServiceUnavailable,
                success: false,
                description:
                    "No se pudo establecer comunicación con Keycloak.",
                CancellationToken.None
            );

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    error = "No se pudo comunicar con Keycloak.",
                    detail = ex.Message
                }
            );
        }
    }

    /// <summary>
    /// Elimina de forma coordinada la identidad del votante en Keycloak y su
    /// perfil electoral en VoterService.
    /// </summary>
    /// <param name="idVotante">
    /// Identificador interno del perfil electoral.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación.
    /// </param>
    /// <returns>
    /// HTTP 204 cuando se elimina, 400 para solicitudes inválidas, 404 cuando
    /// no existe o 503 cuando falla la comunicación.
    /// </returns>
    [HttpDelete("voters/{idVotante:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> DeleteVoterAsync(
        int idVotante,
        CancellationToken ct)
    {
        if (idVotante <= 0)
        {
            return BadRequest(new
            {
                error = "El identificador del votante no es válido."
            });
        }

        try
        {
            var voterProfile =
                await _voterServiceClient.GetProfileByIdAsync(
                    idVotante,
                    ct
                );

            if (string.IsNullOrWhiteSpace(voterProfile.KeycloakId))
            {
                return BadRequest(new
                {
                    error = "El votante no tiene un KeycloakId válido."
                });
            }

            await _keycloakAdminClient.DeleteUserAsync(
                voterProfile.KeycloakId,
                ct
            );

            await _voterServiceClient.DeleteProfileAsync(
                idVotante,
                ct
            );

            await WriteAuditEventAsync(
                action: "VOTER_DELETED",
                entityType: "VoterProfile",
                entityId: idVotante.ToString(),
                username: null,
                httpMethod: HttpMethods.Delete,
                path: $"/api/auth/voters/{idVotante}",
                statusCode: StatusCodes.Status204NoContent,
                success: true,
                description:
                    $"Se eliminó el perfil electoral {idVotante} y su identidad asociada.",
                ct
            );

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                error = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            await WriteAuditEventAsync(
                action: "VOTER_DELETION_FAILED",
                entityType: "VoterProfile",
                entityId: idVotante.ToString(),
                username: null,
                httpMethod: HttpMethods.Delete,
                path: $"/api/auth/voters/{idVotante}",
                statusCode: StatusCodes.Status400BadRequest,
                success: false,
                description: ex.Message,
                CancellationToken.None
            );

            return BadRequest(new
            {
                error = ex.Message
            });
        }
        catch (HttpRequestException ex)
        {
            await WriteAuditEventAsync(
                action: "VOTER_DELETION_FAILED",
                entityType: "VoterProfile",
                entityId: idVotante.ToString(),
                username: null,
                httpMethod: HttpMethods.Delete,
                path: $"/api/auth/voters/{idVotante}",
                statusCode: StatusCodes.Status503ServiceUnavailable,
                success: false,
                description:
                    "No se pudo completar la eliminación distribuida.",
                CancellationToken.None
            );

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    error = "No se pudo completar la eliminación del votante.",
                    detail = ex.Message
                }
            );
        }

        // FIXME: Si Keycloak elimina la identidad y VoterService falla después,
        // el perfil electoral puede quedar huérfano.
    }

    /// <summary>
    /// Registra un nuevo votante creando su identidad en Keycloak y su perfil
    /// electoral en VoterService.
    /// </summary>
    /// <param name="request">
    /// Datos requeridos para registrar al votante.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación.
    /// </param>
    /// <returns>
    /// HTTP 201 con la identidad y el perfil creados; 400 cuando una
    /// dependencia rechaza los datos o 503 cuando falla la comunicación.
    /// </returns>
    [HttpPost("register-voter")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> RegisterVoterAsync(
        RegisterVoterRequest request,
        CancellationToken ct)
    {
        string? keycloakUserId = null;

        try
        {
            keycloakUserId =
                await _keycloakAdminClient.CreateUserAsync(
                    request.Username,
                    request.Nombre,
                    request.CorreoElectronico,
                    request.Password,
                    "VOTANTE",
                    ct
                );

            var authenticatedUserId = User.FindFirst("sub")?.Value;

            var voterProfileRequest =
                new CreateVoterProfileRequest(
                    keycloakUserId,
                    authenticatedUserId,
                    request.Nombre.Trim(),
                    request.Cedula.Trim(),
                    request.CorreoElectronico.Trim()
                );

            var voterProfile =
                await _voterServiceClient.CreateProfileAsync(
                    voterProfileRequest,
                    ct
                );

            await WriteAuditEventAsync(
                action: "VOTER_CREATED",
                entityType: "VoterProfile",
                entityId: keycloakUserId,
                username: request.Username.Trim(),
                httpMethod: HttpMethods.Post,
                path: "/api/auth/register-voter",
                statusCode: StatusCodes.Status201Created,
                success: true,
                description:
                    $"Se registró el votante '{request.Username.Trim()}' y su perfil electoral.",
                ct
            );

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = "Votante registrado correctamente.",
                    keycloakId = keycloakUserId,
                    voterProfile
                }
            );
        }
        catch (InvalidOperationException ex)
        {
            await TryDeleteCompensatingUserAsync(keycloakUserId);

            await WriteAuditEventAsync(
                action: "VOTER_REGISTRATION_FAILED",
                entityType: "VoterProfile",
                entityId: keycloakUserId,
                username: request.Username.Trim(),
                httpMethod: HttpMethods.Post,
                path: "/api/auth/register-voter",
                statusCode: StatusCodes.Status400BadRequest,
                success: false,
                description: ex.Message,
                CancellationToken.None
            );

            return BadRequest(new
            {
                error = ex.Message
            });
        }
        catch (HttpRequestException ex)
        {
            await TryDeleteCompensatingUserAsync(keycloakUserId);

            await WriteAuditEventAsync(
                action: "VOTER_REGISTRATION_FAILED",
                entityType: "VoterProfile",
                entityId: keycloakUserId,
                username: request.Username.Trim(),
                httpMethod: HttpMethods.Post,
                path: "/api/auth/register-voter",
                statusCode: StatusCodes.Status503ServiceUnavailable,
                success: false,
                description:
                    "No se pudo completar el registro distribuido del votante.",
                CancellationToken.None
            );

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    error = "No se pudo completar el registro del votante.",
                    detail = ex.Message
                }
            );
        }
    }

    /// <summary>
    /// Intenta eliminar una identidad creada parcialmente durante el registro.
    /// </summary>
    /// <param name="keycloakUserId">
    /// Identificador de la identidad que debe eliminarse.
    /// </param>
    /// <returns>
    /// Una tarea que representa el intento de compensación.
    /// </returns>
    private async Task TryDeleteCompensatingUserAsync(
        string? keycloakUserId)
    {
        if (string.IsNullOrWhiteSpace(keycloakUserId))
        {
            return;
        }

        try
        {
            await _keycloakAdminClient.DeleteUserAsync(
                keycloakUserId,
                CancellationToken.None
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Falló la eliminación compensatoria del usuario {KeycloakUserId}.",
                keycloakUserId
            );

            await WriteAuditEventAsync(
                action: "VOTER_COMPENSATION_FAILED",
                entityType: "KeycloakUser",
                entityId: keycloakUserId,
                username: null,
                httpMethod: HttpMethods.Delete,
                path: "/admin/realms/votoseguro/users",
                statusCode: StatusCodes.Status500InternalServerError,
                success: false,
                description:
                    "No se pudo eliminar una identidad creada parcialmente.",
                CancellationToken.None
            );
        }
    }

    /// <summary>
    /// Construye y envía un evento funcional hacia AuditService.
    /// </summary>
    /// <param name="action">Acción funcional realizada.</param>
    /// <param name="entityType">Tipo de entidad relacionada.</param>
    /// <param name="entityId">Identificador de la entidad.</param>
    /// <param name="username">Nombre de usuario relacionado.</param>
    /// <param name="httpMethod">Método HTTP.</param>
    /// <param name="path">Ruta procesada.</param>
    /// <param name="statusCode">Código HTTP resultante.</param>
    /// <param name="success">Indica si la operación fue exitosa.</param>
    /// <param name="description">Descripción funcional.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Una tarea que representa el intento de auditoría.</returns>
    private async Task WriteAuditEventAsync(
        string action,
        string entityType,
        string? entityId,
        string? username,
        string httpMethod,
        string path,
        int statusCode,
        bool success,
        string description,
        CancellationToken ct)
    {
        var authenticatedUserId =
            User.FindFirst("sub")?.Value;

        var authenticatedUsername =
            User.Identity?.Name ??
            User.FindFirst("preferred_username")?.Value;

        await _auditClient.TryWriteAsync(
            new CreateAuditEventRequest(
                ServiceName: "AuthService",
                Action: action,
                EntityType: entityType,
                EntityId: entityId,
                UserId: authenticatedUserId,
                Username: authenticatedUsername ?? username,
                HttpMethod: httpMethod,
                Path: path,
                StatusCode: statusCode,
                Success: success,
                Description: description,
                IpAddress:
                    HttpContext.Connection.RemoteIpAddress?.ToString()
            ),
            ct
        );
    }
}