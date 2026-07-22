using System.ComponentModel.DataAnnotations;

namespace VoterService.Models;

/// <summary>
/// Representa los datos requeridos para crear un perfil electoral.
/// </summary>
/// <param name="KeycloakId">
/// UUID de la identidad previamente creada en Keycloak.
/// </param>
/// <param name="Nombre">
/// Nombre completo del votante.
/// </param>
/// <param name="Cedula">
/// Número de cédula compuesto por diez dígitos.
/// </param>
/// <param name="CorreoElectronico">
/// Correo electrónico asociado al perfil electoral.
/// </param>
public sealed record CreateVotanteRequest(
    [Required]
    [MaxLength(255)]
    string KeycloakId,

    [MaxLength(255)]
    string? AdminId,

    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Nombre,

    [Required]
    [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "La cédula debe contener exactamente 10 dígitos."
    )]
    string Cedula,

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string CorreoElectronico
);

/// <summary>
/// Representa los datos modificables de un perfil electoral existente.
/// </summary>
/// <param name="Nombre">
/// Nuevo nombre completo del votante.
/// </param>
/// <param name="Cedula">
/// Nuevo número de cédula compuesto por diez dígitos.
/// </param>
/// <param name="CorreoElectronico">
/// Nuevo correo electrónico asociado al perfil.
/// </param>
public sealed record UpdateVotanteRequest(
    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Nombre,

    [Required]
    [RegularExpression(
        @"^\d{10}$",
        ErrorMessage = "La cédula debe contener exactamente 10 dígitos."
    )]
    string Cedula,

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string CorreoElectronico
);

/// <summary>
/// Representa la información de un perfil electoral devuelta por la API.
/// </summary>
/// <param name="IdVotante">
/// Identificador interno del perfil.
/// </param>
/// <param name="KeycloakId">
/// UUID de la identidad correspondiente en Keycloak.
/// </param>
/// <param name="Nombre">
/// Nombre completo del votante.
/// </param>
/// <param name="Cedula">
/// Número de cédula del votante.
/// </param>
/// <param name="CorreoElectronico">
/// Correo electrónico del votante.
/// </param>
/// <param name="FechaRegistro">
/// Fecha UTC en que se creó el perfil.
/// </param>
public sealed record VotanteResponse(
    int IdVotante,
    string KeycloakId,
    string Nombre,
    string Cedula,
    string CorreoElectronico,
    DateTime FechaRegistro
);