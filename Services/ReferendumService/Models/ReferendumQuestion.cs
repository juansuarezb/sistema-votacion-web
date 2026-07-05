using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReferendumService.Models;

[Table("ReferendumQuestions")]
public class ReferendumQuestion
{
    [Key]
    public int IdQuestion { get; set; }

    public int IdReferendum { get; set; }

    [Required, MaxLength(500)]
    public string Texto { get; set; } = string.Empty;

    public Referendum? Referendum { get; set; }
}