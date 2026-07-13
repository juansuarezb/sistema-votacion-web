using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public sealed class RegisterAdminRequest
{
    [Required]
    [StringLength(50, MinimumLength = 4)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}