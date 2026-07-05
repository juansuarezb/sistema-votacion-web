using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteService.Models;

[Table("Votes")]
public class Vote
{
    [Key]
    public int IdVote { get; set; }

    public int IdReferendum { get; set; }

    public int IdQuestion { get; set; }

    [Required, MaxLength(20)]
    public string TipoVoto { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}