using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

/// <summary>
/// Representa los datos necesarios para registrar la identidad y el perfil
/// electoral de un nuevo votante.
/// </summary>
public sealed class RegisterVoterRequest
{
    /// <summary>
    /// Obtiene o establece el nombre de usuario utilizado para iniciar sesión.
    /// </summary>
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(
        50,
        MinimumLength = 4,
        ErrorMessage = "El nombre de usuario debe tener entre 4 y 50 caracteres."
    )]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el nombre completo del votante.
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres."
    )]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece el número de cédula del votante.
    /// </summary>
    /// <remarks>
    /// La expresión regular valida únicamente que existan diez dígitos.
    /// No valida todavía el dígito verificador de una cédula ecuatoriana.
    /// </remarks>
    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "La cédula debe contener exactamente 10 dígitos."
    )]
    public string Cedula { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del votante.
    /// </summary>
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(
        100,
        ErrorMessage = "El correo electrónico no puede superar los 100 caracteres."
    )]
    public string CorreoElectronico { get; set; } = string.Empty;

    /// <summary>
    /// Obtiene o establece la contraseña inicial del votante.
    /// </summary>
    /// <remarks>
    /// La complejidad definitiva también debe ser aplicada por la política
    /// de contraseñas configurada en Keycloak.
    /// </remarks>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres."
    )]
    public string Password { get; set; } = string.Empty;
}