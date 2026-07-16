using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoterService.Data;
using VoterService.Models;
using VoterService.Services;

namespace VoterService.Controllers;

/// <summary>
/// Expone las operaciones HTTP necesarias para consultar y administrar
/// los perfiles electorales de los votantes.
/// </summary>
/// <remarks>
/// Cada perfil electoral se relaciona lógicamente con una identidad de
/// Keycloak mediante la propiedad <see cref="Votante.KeycloakId"/>.
/// No existe una clave foránea física porque Keycloak y VoterService
/// mantienen almacenamientos independientes.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public sealed class VotantesController : ControllerBase
{
    private readonly VoterDbContext _context;
    private readonly AuditClient _auditClient;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VotantesController"/>.
    /// </summary>
    /// <param name="context">
    /// Contexto utilizado para administrar los perfiles electorales.
    /// </param>
    /// <param name="auditClient">
    /// Cliente utilizado para registrar eventos funcionales en AuditService.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando alguna dependencia es
    /// <see langword="null"/>.
    /// </exception>
    public VotantesController(
        VoterDbContext context,
        AuditClient auditClient)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(auditClient);

        _context = context;
        _auditClient = auditClient;
    }

    /// <summary>
    /// Obtiene todos los perfiles electorales registrados.
    /// </summary>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 que contiene la colección de votantes
    /// registrados. La colección puede estar vacía.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<VotanteResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<IEnumerable<VotanteResponse>>> GetAllAsync(
        CancellationToken ct)
    {
        var votantes = await _context.Votantes
            .AsNoTracking()
            .ToListAsync(ct);

        var response = votantes
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Obtiene un perfil electoral mediante su identificador interno.
    /// </summary>
    /// <param name="id">
    /// Identificador interno del votante en VoterService.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con el perfil encontrado, o 404 cuando no existe
    /// un votante asociado al identificador proporcionado.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VotanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VotanteResponse>> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var votante = await _context.Votantes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                voter => voter.IdVotante == id,
                ct
            );

        if (votante is null)
        {
            return NotFound(new
            {
                mensaje = "Votante no encontrado"
            });
        }

        return Ok(ToResponse(votante));
    }

    /// <summary>
    /// Obtiene el perfil electoral asociado al identificador de una identidad
    /// autenticada en Keycloak.
    /// </summary>
    /// <param name="keycloakId">
    /// Identificador contenido en el claim <c>sub</c> del token JWT emitido
    /// por Keycloak.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con el perfil electoral encontrado; 400 cuando
    /// el identificador está vacío; o 404 cuando no existe un perfil asociado.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpGet("by-keycloak/{keycloakId}")]
    [ProducesResponseType(typeof(VotanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VotanteResponse>> GetByKeycloakIdAsync(
        string keycloakId,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(keycloakId))
        {
            return BadRequest(new
            {
                error = "El KeycloakId es obligatorio."
            });
        }

        var votante = await _context.Votantes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                voter => voter.KeycloakId == keycloakId,
                ct
            );

        if (votante is null)
        {
            return NotFound(new
            {
                mensaje =
                    "No existe un perfil de votante asociado al usuario autenticado."
            });
        }

        return Ok(ToResponse(votante));
    }

    /// <summary>
    /// Crea un nuevo perfil electoral asociado a una identidad de Keycloak.
    /// </summary>
    /// <param name="request">
    /// Datos necesarios para crear el perfil electoral.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 200 con el perfil creado, o 409 cuando ya existe
    /// otro votante con la misma identidad, cédula o correo electrónico.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// Se produce si SQL Server rechaza la inserción.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpPost]
    [ProducesResponseType(typeof(VotanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VotanteResponse>> CreateAsync(
        CreateVotanteRequest request,
        CancellationToken ct)
    {
        var keycloakId = request.KeycloakId.Trim();
        var nombre = request.Nombre.Trim();
        var cedula = request.Cedula.Trim();
        var correoElectronico = request.CorreoElectronico.Trim();

        var existe = await _context.Votantes.AnyAsync(
            voter =>
                voter.Cedula == cedula ||
                voter.CorreoElectronico == correoElectronico ||
                voter.KeycloakId == keycloakId,
            ct
        );

        if (existe)
        {
            return Conflict(new
            {
                error =
                    "Ya existe un votante con esa identidad, cédula o correo."
            });
        }

        var votante = new Votante
        {
            KeycloakId = keycloakId,
            Nombre = nombre,
            Cedula = cedula,
            CorreoElectronico = correoElectronico
        };

        _context.Votantes.Add(votante);
        await _context.SaveChangesAsync(ct);

        return Ok(ToResponse(votante));
    }

    /// <summary>
    /// Actualiza los datos personales de un perfil electoral existente.
    /// </summary>
    /// <param name="id">
    /// Identificador interno del votante que debe actualizarse.
    /// </param>
    /// <param name="request">
    /// Nuevos datos personales del perfil electoral.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 204 cuando la actualización se completa; 404 cuando
    /// el votante no existe; o 409 cuando la cédula o el correo ya están
    /// asociados a otro perfil.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// Se produce si SQL Server rechaza la actualización.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        UpdateVotanteRequest request,
        CancellationToken ct)
    {
        var votante = await _context.Votantes.FindAsync(
            [id],
            ct
        );

        if (votante is null)
        {
            return NotFound(new
            {
                mensaje = "Votante no encontrado"
            });
        }

        var nombreNormalizado = request.Nombre.Trim();
        var cedulaNormalizada = request.Cedula.Trim();
        var correoNormalizado =
            request.CorreoElectronico.Trim();

        var duplicado = await _context.Votantes.AnyAsync(
            voter =>
                voter.IdVotante != id &&
                (
                    voter.Cedula == cedulaNormalizada ||
                    voter.CorreoElectronico == correoNormalizado
                ),
            ct
        );

        if (duplicado)
        {
            return Conflict(new
            {
                error =
                    "Ya existe otro votante con esa cédula o correo."
            });
        }

        votante.Nombre = nombreNormalizado;
        votante.Cedula = cedulaNormalizada;
        votante.CorreoElectronico = correoNormalizado;

        await _context.SaveChangesAsync(ct);

        await WriteAuditEventAsync(
            action: "VOTER_UPDATED",
            entityType: "VoterProfile",
            entityId: id.ToString(),
            httpMethod: HttpMethods.Put,
            path: $"/api/votantes/{id}",
            statusCode: StatusCodes.Status204NoContent,
            description:
                $"Se actualizaron los datos del perfil electoral {id}.",
            ct
        );

        // TODO: La actualización modifica únicamente VoterService.
        // Implementar una operación coordinada mediante AuthService para
        // sincronizar también nombre y correo en Keycloak.
        return NoContent();
    }

    /// <summary>
    /// Elimina un perfil electoral de la base de datos de VoterService.
    /// </summary>
    /// <param name="id">
    /// Identificador interno del perfil electoral que debe eliminarse.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación asíncrona.
    /// </param>
    /// <returns>
    /// Un resultado HTTP 204 cuando el perfil se elimina o 404 cuando el
    /// votante no existe.
    /// </returns>
    /// <exception cref="DbUpdateException">
    /// Se produce si SQL Server rechaza la eliminación.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Se produce si la operación es cancelada mediante
    /// <paramref name="ct"/>.
    /// </exception>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int id,
        CancellationToken ct)
    {
        var votante = await _context.Votantes.FindAsync(
            [id],
            ct
        );

        if (votante is null)
        {
            return NotFound(new
            {
                mensaje = "Votante no encontrado"
            });
        }

        // Este endpoint elimina únicamente el perfil. La eliminación pública
        // coordinada debe realizarse mediante AuthService para borrar también
        // la identidad correspondiente en Keycloak.
        _context.Votantes.Remove(votante);
        await _context.SaveChangesAsync(ct);

        return NoContent();
    }

    /// <summary>
    /// Convierte una entidad persistida en el DTO expuesto por la API.
    /// </summary>
    /// <param name="votante">
    /// Entidad de dominio que debe convertirse.
    /// </param>
    /// <returns>
    /// Una instancia de <see cref="VotanteResponse"/> con los datos públicos
    /// del perfil electoral.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando <paramref name="votante"/> es
    /// <see langword="null"/>.
    /// </exception>
    private static VotanteResponse ToResponse(
        Votante votante)
    {
        ArgumentNullException.ThrowIfNull(votante);

        return new VotanteResponse(
            votante.IdVotante,
            votante.KeycloakId,
            votante.Nombre,
            votante.Cedula,
            votante.CorreoElectronico,
            votante.FechaRegistro
        );
    }

    /// <summary>
    /// Construye y envía un evento funcional hacia AuditService.
    /// </summary>
    /// <param name="action">
    /// Acción funcional realizada.
    /// </param>
    /// <param name="entityType">
    /// Tipo de entidad afectada.
    /// </param>
    /// <param name="entityId">
    /// Identificador de la entidad afectada.
    /// </param>
    /// <param name="httpMethod">
    /// Método HTTP asociado con la operación.
    /// </param>
    /// <param name="path">
    /// Ruta HTTP procesada.
    /// </param>
    /// <param name="statusCode">
    /// Código HTTP resultante.
    /// </param>
    /// <param name="description">
    /// Descripción funcional del evento.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación.
    /// </param>
    /// <returns>
    /// Una tarea que representa el intento de registro en AuditService.
    /// </returns>
    private async Task WriteAuditEventAsync(
        string action,
        string entityType,
        string? entityId,
        string httpMethod,
        string path,
        int statusCode,
        string description,
        CancellationToken ct)
    {
        var userId =
            User.FindFirst("sub")?.Value;

        var username =
            User.Identity?.Name ??
            User.FindFirst("preferred_username")?.Value;

        await _auditClient.TryWriteAsync(
            new CreateAuditEventRequest(
                ServiceName: "VoterService",
                Action: action,
                EntityType: entityType,
                EntityId: entityId,
                UserId: userId,
                Username: username,
                HttpMethod: httpMethod,
                Path: path,
                StatusCode: statusCode,
                Success: true,
                Description: description,
                IpAddress:
                    HttpContext.Connection.RemoteIpAddress?.ToString()
            ),
            ct
        );
    }
}