using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReferendumService.Models;

/// <summary>
/// Representa un candidato asociado a una pregunta de un referéndum.
/// </summary>
[Table("ReferendumQuestionCandidates", Schema = "referendum")]
public sealed class ReferendumQuestionCandidate
{
    /// <summary>
    /// Obtiene o establece el identificador único del candidato.
    /// </summary>
    [Key]
    public int IdCandidate { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador de la pregunta asociada.
    /// </summary>
    public int IdQuestion { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre del candidato.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la URL de la imagen del candidato.
    /// </summary>
    [MaxLength(1000)]
    public string? ImagenUrl { get; set; }

    /// <summary>
    /// Obtiene o establece la pregunta a la que pertenece este candidato.
    /// </summary>
    [ForeignKey(nameof(IdQuestion))]
    public ReferendumQuestion? Question { get; set; }
}
