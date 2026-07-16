using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteService.Models;

/// <summary>
/// Representa un voto anónimo registrado para una pregunta de un referéndum.
/// </summary>
/// <remarks>
/// La entidad no almacena el identificador del votante para evitar relacionar
/// directamente una identidad con su selección. La prevención de doble voto
/// se controla mediante las asignaciones administradas por ReferendumService.
/// </remarks>
[Table("Votes", Schema = "vote")]
public sealed class Vote
{
    /// <summary>
    /// Obtiene o establece el identificador interno del voto.
    /// </summary>
    [Key]
    public int IdVote { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador del referéndum.
    /// </summary>
    public int IdReferendum { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador de la pregunta respondida.
    /// </summary>
    public int IdQuestion { get; set; }

    /// <summary>
    /// Obtiene o establece el tipo de voto registrado.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string TipoVoto { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la fecha UTC de registro del voto.
    /// </summary>
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}