namespace VoterService.Models;

public record CreateVotanteRequest(
    string KeycloakId, 
    string Nombre, 
    string Cedula, 
    string CorreoElectronico
);

public record UpdateVotanteRequest(
    string Nombre, 
    string Cedula, 
    string CorreoElectronico
);

public record VotanteResponse(
    int IdVotante,
    string KeycloakId,
    string Nombre,
    string Cedula,
    string CorreoElectronico,
    DateTime FechaRegistro
);