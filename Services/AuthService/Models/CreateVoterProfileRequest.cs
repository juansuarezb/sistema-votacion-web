namespace AuthService.Models;

public sealed record CreateVoterProfileRequest(
    string KeycloakId,
    string Nombre,
    string Cedula,
    string CorreoElectronico
);