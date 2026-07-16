namespace AuthService.Models;

/// <summary>
/// Representa un evento enviado desde AuthService hacia AuditService.
/// </summary>
/// <param name="ServiceName">
/// Nombre del servicio que originó el evento.
/// </param>
/// <param name="Action">
/// Acción funcional realizada.
/// </param>
/// <param name="EntityType">
/// Tipo de entidad afectada.
/// </param>
/// <param name="EntityId">
/// Identificador de la entidad relacionada.
/// </param>
/// <param name="UserId">
/// Identificador del usuario que ejecutó la acción, cuando esté disponible.
/// </param>
/// <param name="Username">
/// Nombre del usuario que ejecutó la acción, cuando esté disponible.
/// </param>
/// <param name="HttpMethod">
/// Método HTTP asociado con la operación.
/// </param>
/// <param name="Path">
/// Ruta HTTP procesada.
/// </param>
/// <param name="StatusCode">
/// Código de estado HTTP resultante.
/// </param>
/// <param name="Success">
/// Indica si la operación terminó correctamente.
/// </param>
/// <param name="Description">
/// Descripción funcional del evento.
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