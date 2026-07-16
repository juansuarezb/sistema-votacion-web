using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReferendumService.Models;

/// <summary>
/// Representa un proceso de referéndum administrado por VotoSeguro.
/// </summary>
[Table("Referendums", Schema = "referendum")]
public sealed class Referendum
{
    /// <summary>
    /// Obtiene o establece el identificador interno del referéndum.
    /// </summary>
    [Key]
    public int IdReferendum { get; set; }

    /// <summary>
    /// Obtiene o establece el título visible del referéndum.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la descripción opcional del referéndum.
    /// </summary>
    [MaxLength(1000)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha UTC desde la cual se permite votar.
    /// </summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha UTC hasta la cual se permite votar.
    /// </summary>
    public DateTime FechaCierre { get; set; }

    /// <summary>
    /// Obtiene o establece el estado operativo del referéndum.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string Estado { get; set; } = "BORRADOR";

    /// <summary>
    /// Obtiene o establece la fecha UTC de creación.
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Obtiene las preguntas asociadas al referéndum.
    /// </summary>
    public ICollection<ReferendumQuestion> Preguntas { get; set; } =
        new List<ReferendumQuestion>();
}