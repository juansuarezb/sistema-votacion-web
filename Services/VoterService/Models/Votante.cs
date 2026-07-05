using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoterService.Models;

[Table("Votantes")]
public class Votante
{
    [Key]
    public int IdVotante { get; set; }

    [Required, MaxLength(255)]
    public string KeycloakId { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string Cedula { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string CorreoElectronico { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}