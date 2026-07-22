using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoterService.Models;

/// <summary>
/// Representa el perfil electoral de un votante almacenado por VoterService.
/// </summary>
/// <remarks>
/// La autenticación y las credenciales pertenecen a Keycloak. Esta entidad
/// almacena únicamente la información de dominio necesaria para participar
/// en los procesos electorales.
/// </remarks>
[Table("Votantes", Schema = "voter")]
public sealed class Votante
{
    /// <summary>
    /// Obtiene o establece el identificador interno del perfil electoral.
    /// </summary>
    [Key]
    public int IdVotante { get; set; }

    /// <summary>
    /// Obtiene o establece el UUID de la identidad correspondiente en
    /// Keycloak.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string KeycloakId { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el UUID de la identidad del administrador (en Keycloak) que creó a este votante.
    /// Permite el aislamiento de datos (multitenancy por administrador).
    /// </summary>
    [MaxLength(255)]
    public string? AdminId { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre completo del votante.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el número de cédula del votante.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Cedula { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el correo electrónico asociado al perfil.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string CorreoElectronico { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la fecha UTC en que se registró el perfil.
    /// </summary>
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}