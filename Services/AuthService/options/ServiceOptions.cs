using System.ComponentModel.DataAnnotations;

namespace AuthService.Options;

/// <summary>
/// Contiene las direcciones internas de los microservicios utilizados por
/// AuthService.
/// </summary>
public sealed class ServiceOptions
{
    /// <summary>
    /// Obtiene o establece la dirección interna de VoterService.
    /// </summary>
    [Required]
    [Url]
    public string VoterServiceUrl { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la dirección interna de AuditService.
    /// </summary>
    [Required]
    [Url]
    public string AuditServiceUrl { get; set; } = string.Empty;
}