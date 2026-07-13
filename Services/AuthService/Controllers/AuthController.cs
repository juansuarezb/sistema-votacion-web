using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly KeycloakAdminClient _keycloakAdminClient;
    private readonly VoterServiceClient _voterServiceClient;

    public AuthController(
        KeycloakAdminClient keycloakAdminClient,
        VoterServiceClient voterServiceClient)
    {
        _keycloakAdminClient = keycloakAdminClient;
        _voterServiceClient = voterServiceClient;
    }

    /// <summary>
    /// POST /api/auth/register-admin
    /// Registra un nuevo administrador en Keycloak
    /// y le asigna el rol ADMIN.
    /// </summary>
    [HttpPost("register-admin")]
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
            return BadRequest(new
            {
                error = ex.Message
            });
        }
        catch (HttpRequestException ex)
        {
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
    /// DELETE /api/auth/voters/{idVotante}
    /// Elimina el usuario en Keycloak y el perfil en VoterService.
    /// </summary>
    [HttpDelete("voters/{idVotante:int}")]
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
            // 1. Consultar el perfil para obtener el KeycloakId.
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

            // 2. Eliminar primero la identidad.
            // Así el usuario deja de poder iniciar sesión inmediatamente.
            await _keycloakAdminClient.DeleteUserAsync(
                voterProfile.KeycloakId,
                ct
            );

            // 3. Eliminar el perfil electoral.
            await _voterServiceClient.DeleteProfileAsync(
                idVotante,
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
            return BadRequest(new
            {
                error = ex.Message
            });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    error = "No se pudo completar la eliminación del votante.",
                    detail = ex.Message
                }
            );
        }
    }

    /// <summary>
    /// POST /api/auth/register-voter
    /// Crea el usuario en Keycloak, asigna el rol VOTANTE
    /// y luego crea el perfil en VoterService.
    /// </summary>
    [HttpPost("register-voter")]
    public async Task<IActionResult> RegisterVoterAsync(
        RegisterVoterRequest request,
        CancellationToken ct)
    {
        string? keycloakUserId = null;

        try
        {
            // 1. Crear usuario en Keycloak y asignar rol VOTANTE
            keycloakUserId =
                await _keycloakAdminClient.CreateUserAsync(
                    request.Username,
                    request.Nombre,
                    request.CorreoElectronico,
                    request.Password,
                    "VOTANTE",
                    ct
                );

            // 2. Preparar el perfil electoral para VoterService
            var voterProfileRequest =
                new CreateVoterProfileRequest(
                    keycloakUserId,
                    request.Nombre.Trim(),
                    request.Cedula.Trim(),
                    request.CorreoElectronico.Trim()
                );

            // 3. Crear perfil en VoterService
            var voterProfile =
                await _voterServiceClient.CreateProfileAsync(
                    voterProfileRequest,
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
            // Si Keycloak creó el usuario pero VoterService falló,
            // se elimina el usuario para evitar datos inconsistentes.
            if (!string.IsNullOrWhiteSpace(keycloakUserId))
            {
                try
                {
                    await _keycloakAdminClient.DeleteUserAsync(
                        keycloakUserId,
                        CancellationToken.None
                    );
                }
                catch
                {
                    // No reemplazamos el error original.
                    // Más adelante esto debería enviarse a logs.
                }
            }

            return BadRequest(new
            {
                error = ex.Message
            });
        }
        catch (HttpRequestException ex)
        {
            if (!string.IsNullOrWhiteSpace(keycloakUserId))
            {
                try
                {
                    await _keycloakAdminClient.DeleteUserAsync(
                        keycloakUserId,
                        CancellationToken.None
                    );
                }
                catch
                {
                    // No reemplazamos el error original.
                }
            }

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
}