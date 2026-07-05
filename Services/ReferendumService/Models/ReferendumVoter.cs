using System.ComponentModel.DataAnnotations.Schema;

namespace ReferendumService.Models;

[Table("ReferendumQuestionVoters")]
public class ReferendumQuestionVoter
{
    public int IdReferendum { get; set; }

    public int IdQuestion { get; set; }

    public int IdVotante { get; set; }

    public bool HaVotado { get; set; } = false;

    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaVoto { get; set; }

    public Referendum? Referendum { get; set; }

    public ReferendumQuestion? Question { get; set; }
}