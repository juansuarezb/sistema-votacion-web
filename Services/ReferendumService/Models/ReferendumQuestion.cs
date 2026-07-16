using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReferendumService.Models;

/// <summary>
/// Representa una pregunta perteneciente a un referéndum.
/// </summary>
[Table("ReferendumQuestions", Schema = "referendum")]
public sealed class ReferendumQuestion
{
    /// <summary>
    /// Obtiene o establece el identificador interno de la pregunta.
    /// </summary>
    [Key]
    public int IdQuestion { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador del referéndum propietario.
    /// </summary>
    public int IdReferendum { get; set; }

    /// <summary>
    /// Obtiene o establece el contenido textual de la pregunta.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Texto { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el referéndum al que pertenece la pregunta.
    /// </summary>
    public Referendum? Referendum { get; set; }
}