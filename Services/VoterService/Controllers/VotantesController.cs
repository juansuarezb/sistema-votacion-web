using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoterService.Data;
using VoterService.Models;

namespace VoterService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VotantesController : ControllerBase
{
    private readonly VoterDbContext _context;

    public VotantesController(VoterDbContext context) => _context = context;

    /// <summary>
    /// GET /api/votantes - Listar todos los votantes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VotanteResponse>>> GetAllAsync(CancellationToken ct)
    {
        var votantes = await _context.Votantes
            .AsNoTracking()
            .ToListAsync(ct);

        var response = votantes.Select(v => new VotanteResponse(
            v.IdVotante,
            v.KeycloakId,
            v.Nombre,
            v.Cedula,
            v.CorreoElectronico,
            v.FechaRegistro
        )).ToList();

        return Ok(response);
    }

    /// <summary>
    /// GET /api/votantes/{id} - Obtener votante por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VotanteResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var votante = await _context.Votantes
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.IdVotante == id, ct);

        if (votante is null)
            return NotFound(new { mensaje = "Votante no encontrado" });

        var response = new VotanteResponse(
            votante.IdVotante,
            votante.KeycloakId,
            votante.Nombre,
            votante.Cedula,
            votante.CorreoElectronico,
            votante.FechaRegistro
        );

        return Ok(response);
    }

    /// <summary>
    /// GET /api/votantes/by-keycloak/{keycloakId}
    /// Obtener el perfil del votante mediante el sub de Keycloak.
    /// </summary>
    [HttpGet("by-keycloak/{keycloakId}")]
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
                v => v.KeycloakId == keycloakId,
                ct
            );

        if (votante is null)
        {
            return NotFound(new
            {
                mensaje = "No existe un perfil de votante asociado al usuario autenticado."
            });
        }

        return Ok(new VotanteResponse(
            votante.IdVotante,
            votante.KeycloakId,
            votante.Nombre,
            votante.Cedula,
            votante.CorreoElectronico,
            votante.FechaRegistro
        ));
    }

    /// <summary>
    /// POST /api/votantes - Crear nuevo votante
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VotanteResponse>> CreateAsync(
        CreateVotanteRequest request,
        CancellationToken ct)
    {
        // Validación de duplicados
        var existe = await _context.Votantes.AnyAsync(
            v => v.Cedula == request.Cedula || v.CorreoElectronico == request.CorreoElectronico,
            ct);

        if (existe)
            return Conflict(new { error = "Ya existe un votante con esa cédula o correo" });

        var votante = new Votante
        {
            KeycloakId = request.KeycloakId,
            Nombre = request.Nombre,
            Cedula = request.Cedula,
            CorreoElectronico = request.CorreoElectronico
        };

        _context.Votantes.Add(votante);
        await _context.SaveChangesAsync(ct);

        var response = new VotanteResponse(
            votante.IdVotante,
            votante.KeycloakId,
            votante.Nombre,
            votante.Cedula,
            votante.CorreoElectronico,
            votante.FechaRegistro
        );

        return Ok(response);
    }

    /// <summary>
    /// PUT /api/votantes/{id} - Actualizar votante
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(
        int id,
        UpdateVotanteRequest request,
        CancellationToken ct)
    {
        var votante = await _context.Votantes.FindAsync(new object[] { id }, ct);
        if (votante is null)
            return NotFound(new { mensaje = "Votante no encontrado" });

        // Validar que no haya duplicado en cédula o email (si cambian)
        var duplicado = await _context.Votantes.AnyAsync(
            v => v.IdVotante != id &&
                (v.Cedula == request.Cedula || v.CorreoElectronico == request.CorreoElectronico),
            ct);

        if (duplicado)
            return Conflict(new { error = "Ya existe otro votante con esa cédula o correo" });

        votante.Nombre = request.Nombre;
        votante.Cedula = request.Cedula;
        votante.CorreoElectronico = request.CorreoElectronico;

        await _context.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>
    /// DELETE /api/votantes/{id} - Eliminar votante
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct)
    {
        var votante = await _context.Votantes.FindAsync(new object[] { id }, ct);
        if (votante is null)
            return NotFound(new { mensaje = "Votante no encontrado" });

        _context.Votantes.Remove(votante);
        await _context.SaveChangesAsync(ct);
        return NoContent();
    }
}