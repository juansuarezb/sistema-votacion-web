namespace AuthService.Models;

/// <summary>
/// Representa la solicitud interna enviada desde AuthService hacia
/// VoterService para crear el perfil electoral de un votante.
/// </summary>
/// <param name="KeycloakId">
/// Identificador UUID generado por Keycloak para la identidad del votante.
/// </param>
/// <param name="Nombre">
/// Nombre completo del votante.
/// </param>
/// <param name="Cedula">
/// Número de cédula del votante.
/// </param>
/// <param name="CorreoElectronico">
/// Dirección de correo electrónico asociada al perfil electoral.
/// </param>
public sealed record CreateVoterProfileRequest(
    string KeycloakId,
    string Nombre,
    string Cedula,
    string CorreoElectronico
);