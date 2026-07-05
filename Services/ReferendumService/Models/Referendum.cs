using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReferendumService.Models;

[Table("Referendums")]
public class Referendum
{
    [Key]
    public int IdReferendum { get; set; }

    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descripcion { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaCierre { get; set; }

    [Required, MaxLength(30)]
    public string Estado { get; set; } = "BORRADOR";

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<ReferendumQuestion> Preguntas { get; set; } = new List<ReferendumQuestion>();
}