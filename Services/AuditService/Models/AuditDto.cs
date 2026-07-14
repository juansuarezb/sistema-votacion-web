using System.ComponentModel.DataAnnotations;

namespace AuditService.Models;

/// <summary>
/// Representa una solicitud para registrar un evento de auditoría.
/// </summary>
public sealed class CreateAuditEventRequest
{
    [Required]
    [StringLength(100)]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EntityType { get; set; } = string.Empty;

    [StringLength(100)]
    public string? EntityId { get; set; }

    [StringLength(255)]
    public string? UserId { get; set; }

    [StringLength(100)]
    public string? Username { get; set; }

    [Required]
    [StringLength(10)]
    public string HttpMethod { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Path { get; set; } = string.Empty;

    [Range(100, 599)]
    public int StatusCode { get; set; }

    public bool Success { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(64)]
    public string? IpAddress { get; set; }
}

/// <summary>
/// Representa un evento de auditoría devuelto por la API.
/// </summary>
public sealed record AuditEventResponse(
    long IdAuditEvent,
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
    string? IpAddress,
    DateTime OccurredAtUtc
);