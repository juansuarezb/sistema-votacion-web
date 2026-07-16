using System.ComponentModel.DataAnnotations.Schema;

namespace ReferendumService.Models;

/// <summary>
/// Representa la asignación de un votante a una pregunta determinada de un
/// referéndum y conserva su estado de participación.
/// </summary>
[Table("ReferendumQuestionVoters", Schema = "referendum")]
public sealed class ReferendumQuestionVoter
{
    /// <summary>
    /// Obtiene o establece el identificador del referéndum asignado.
    /// </summary>
    public int IdReferendum { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador de la pregunta asignada.
    /// </summary>
    public int IdQuestion { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador interno del votante.
    /// </summary>
    /// <remarks>
    /// Este identificador pertenece a VoterService. No existe una clave
    /// foránea física porque cada microservicio administra su propia base.
    /// </remarks>
    public int IdVotante { get; set; }

    /// <summary>
    /// Obtiene o establece si el votante ya respondió la pregunta.
    /// </summary>
    public bool HaVotado { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha UTC de asignación.
    /// </summary>
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Obtiene o establece la fecha UTC en que se registró la respuesta.
    /// </summary>
    public DateTime? FechaVoto { get; set; }

    /// <summary>
    /// Obtiene o establece el referéndum asociado.
    /// </summary>
    public Referendum? Referendum { get; set; }

    /// <summary>
    /// Obtiene o establece la pregunta asociada.
    /// </summary>
    public ReferendumQuestion? Question { get; set; }
}