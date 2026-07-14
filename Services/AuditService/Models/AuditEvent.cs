using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditService.Models;

/// <summary>
/// Representa un evento auditable generado por alguno de los componentes
/// de VotoSeguro.
/// </summary>
[Table("AuditEvents", Schema = "audit")]
public sealed class AuditEvent
{
    /// <summary>
    /// Identificador único del evento de auditoría.
    /// </summary>
    [Key]
    public long IdAuditEvent { get; set; }

    /// <summary>
    /// Nombre del servicio que originó el evento.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Acción funcional realizada.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de entidad afectada.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Identificador de la entidad afectada.
    /// </summary>
    [MaxLength(100)]
    public string? EntityId { get; set; }

    /// <summary>
    /// Identificador del usuario autenticado, normalmente el sub de Keycloak.
    /// </summary>
    [MaxLength(255)]
    public string? UserId { get; set; }

    /// <summary>
    /// Nombre de usuario visible.
    /// </summary>
    [MaxLength(100)]
    public string? Username { get; set; }

    /// <summary>
    /// Método HTTP asociado con la operación.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string HttpMethod { get; set; } = string.Empty;

    /// <summary>
    /// Ruta HTTP procesada.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Código de estado HTTP resultante.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Indica si la operación terminó correctamente.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Descripción legible del evento.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Dirección IP del cliente que originó la operación.
    /// </summary>
    [MaxLength(64)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Fecha UTC en la que ocurrió el evento.
    /// </summary>
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}