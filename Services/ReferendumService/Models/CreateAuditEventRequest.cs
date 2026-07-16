namespace ReferendumService.Models;

/// <summary>
/// Representa un evento enviado desde ReferendumService hacia AuditService.
/// </summary>
/// <param name="ServiceName">Servicio que originó el evento.</param>
/// <param name="Action">Acción funcional realizada.</param>
/// <param name="EntityType">Tipo de entidad afectada.</param>
/// <param name="EntityId">Identificador de la entidad afectada.</param>
/// <param name="UserId">Identificador del usuario, cuando esté disponible.</param>
/// <param name="Username">Nombre del usuario, cuando esté disponible.</param>
/// <param name="HttpMethod">Método HTTP asociado.</param>
/// <param name="Path">Ruta HTTP procesada.</param>
/// <param name="StatusCode">Código HTTP resultante.</param>
/// <param name="Success">Indica si la operación fue exitosa.</param>
/// <param name="Description">Descripción funcional del evento.</param>
/// <param name="IpAddress">Dirección IP del cliente.</param>
public sealed record CreateAuditEventRequest(
    string ServiceName,
    string Action,
    string EntityType,
    string? EntityId,
    string? UserId,
    string? Username,
    string HttpMethod,
    string Path,
    int StatusCode,
    bool Success,
    string? Description,
    string? IpAddress
);