namespace VoteService.Models;

/// <summary>
/// Representa un evento enviado desde VoteService hacia AuditService.
/// </summary>
/// <param name="ServiceName">
/// Nombre del servicio que originó el evento.
/// </param>
/// <param name="Action">
/// Acción funcional ejecutada.
/// </param>
/// <param name="EntityType">
/// Tipo de entidad relacionada con el evento.
/// </param>
/// <param name="EntityId">
/// Identificador no sensible de la entidad.
/// </param>
/// <param name="UserId">
/// Identificador del usuario, cuando corresponda.
/// </param>
/// <param name="Username">
/// Nombre de usuario visible, cuando corresponda.
/// </param>
/// <param name="HttpMethod">
/// Método HTTP asociado con la operación.
/// </param>
/// <param name="Path">
/// Ruta HTTP procesada.
/// </param>
/// <param name="StatusCode">
/// Código HTTP resultante.
/// </param>
/// <param name="Success">
/// Indica si la operación finalizó correctamente.
/// </param>
/// <param name="Description">
/// Descripción funcional y no sensible del evento.
/// </param>
/// <param name="IpAddress">
/// Dirección IP del cliente, cuando esté disponible.
/// </param>
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