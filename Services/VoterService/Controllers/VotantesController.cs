using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoterService.Data;
using VoterService.Models;
using VoterService.Services;

namespace VoterService.Controllers;


// Expone las operaciones HTTP necesarias para consultar y administrar
// los perfiles electorales de los votantes.

[ApiController]
[Route("api/[controller]")]
public sealed class VotantesController : ControllerBase
{
    private readonly VoterDbContext _context;
    private readonly AuditClient _auditClient;

  
    //Inicializa una nueva instancia de <see cref="VotantesController"/>.
    public VotantesController(
        VoterDbContext context,
        AuditClient auditClient)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(auditClient);

        _context = context;
        _auditClient = auditClient;
    }

   
    // Obtiene todos los perfiles electorales registrados.
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<VotanteResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<IEnumerable<VotanteResponse>>> GetAllAsync(
        CancellationToken ct)
    {
        var adminId = User.FindFirst("sub")?.Value;

        var query = _context.Votantes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(adminId))
        {
            query = query.Where(v => v.AdminId == adminId);
        }

        var votantes = await query.ToListAsync(ct);

        var response = votantes
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

   
    // Obtiene un perfil electoral mediante su identificador interno.

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

   
    // Obtiene el perfil electoral asociado al identificador de una identidad
    // autenticada en Keycloak.
  
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

  
    // Crea un nuevo perfil electoral asociado a una identidad de Keycloak.
    [HttpPost]
    [ProducesResponseType(typeof(VotanteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VotanteResponse>> CreateAsync(
        CreateVotanteRequest request,
        CancellationToken ct)
    {
        var keycloakId = request.KeycloakId.Trim();
        var adminId = request.AdminId?.Trim();
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
            AdminId = adminId,
            Nombre = nombre,
            Cedula = cedula,
            CorreoElectronico = correoElectronico
        };

        _context.Votantes.Add(votante);
        await _context.SaveChangesAsync(ct);

        return Ok(ToResponse(votante));
    }

  
    // Actualiza los datos personales de un perfil electoral existente.
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

    // Elimina un perfil electoral de la base de datos de VoterService.
 
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


// Convierte una entidad persistida en el DTO expuesto por la API.
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

  
    // Construye y envía un evento funcional hacia AuditService.
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