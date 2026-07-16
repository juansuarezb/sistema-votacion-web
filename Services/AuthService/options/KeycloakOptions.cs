using System.ComponentModel.DataAnnotations;

namespace AuthService.Options;

/// <summary>
/// Representa la configuración necesaria para que AuthService se comunique
/// con Keycloak mediante la API administrativa y el flujo Client Credentials.
/// </summary>
public sealed class KeycloakOptions
{
    /// <summary>
    /// Obtiene o establece la URL base interna de Keycloak.
    /// </summary>
    /// <example>
    /// http://keycloak:8080
    /// </example>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el nombre del realm administrado por AuthService.
    /// </summary>
    [Required]
    public string Realm { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el identificador del cliente técnico utilizado
    /// para obtener tokens mediante Client Credentials.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el secreto del cliente técnico configurado
    /// en Keycloak.
    /// </summary>
    /// <remarks>
    /// Este valor debe proporcionarse mediante variables de entorno o un
    /// mecanismo seguro de gestión de secretos en entornos no académicos.
    /// </remarks>
    [Required]
    public string ClientSecret { get; set; } = string.Empty;
}