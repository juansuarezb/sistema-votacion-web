using AuditService.Data;
using AuditService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Controllers;


// Expone operaciones para registrar y consultar eventos de auditoría.

[ApiController]
[Route("api/audit/events")]
public sealed class AuditEventsController : ControllerBase
{
    private readonly AuditDbContext _context;


    // Inicializa el controlador de auditoría.
   
    public AuditEventsController(AuditDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

   
    // Registra un nuevo evento de auditoría.
  
    [HttpPost]
    [ProducesResponseType(
        typeof(AuditEventResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuditEventResponse>> CreateAsync(
        CreateAuditEventRequest request,
        CancellationToken ct)
    {
        var auditEvent = new AuditEvent
        {
            ServiceName = request.ServiceName.Trim(),
            Action = request.Action.Trim(),
            EntityType = request.EntityType.Trim(),
            EntityId = request.EntityId?.Trim(),
            UserId = request.UserId?.Trim(),
            Username = request.Username?.Trim(),
            HttpMethod = request.HttpMethod.Trim().ToUpperInvariant(),
            Path = request.Path.Trim(),
            StatusCode = request.StatusCode,
            Success = request.Success,
            Description = request.Description?.Trim(),
            IpAddress = request.IpAddress?.Trim(),
            OccurredAtUtc = DateTime.UtcNow
        };

        _context.AuditEvents.Add(auditEvent);
        await _context.SaveChangesAsync(ct);

        var response = ToResponse(auditEvent);

        return StatusCode(
            StatusCodes.Status201Created,
            response
        );
    }

   
    // Obtiene los eventos de auditoría más recientes.
   
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<AuditEventResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuditEventResponse>>>
        GetAllAsync(
            [FromQuery] int limit = 100,
            CancellationToken ct = default)
    {
        var safeLimit = Math.Clamp(limit, 1, 500);

        var events = await _context.AuditEvents
            .AsNoTracking()
            .OrderByDescending(item => item.OccurredAtUtc)
            .Take(safeLimit)
            .ToListAsync(ct);

        return Ok(events.Select(ToResponse));
    }

    
    // Obtiene un evento de auditoría por identificador.
    
    [HttpGet("{id:long}")]
    [ProducesResponseType(
        typeof(AuditEventResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuditEventResponse>> GetByIdAsync(
        long id,
        CancellationToken ct)
    {
        var auditEvent = await _context.AuditEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.IdAuditEvent == id,
                ct
            );

        if (auditEvent is null)
        {
            return NotFound(new
            {
                error = "Evento de auditoría no encontrado."
            });
        }

        return Ok(ToResponse(auditEvent));
    }

  
    // Convierte una entidad de auditoría en su representación pública.
    private static AuditEventResponse ToResponse(
        AuditEvent auditEvent)
    {
        return new AuditEventResponse(
            auditEvent.IdAuditEvent,
            auditEvent.ServiceName,
            auditEvent.Action,
            auditEvent.EntityType,
            auditEvent.EntityId,
            auditEvent.UserId,
            auditEvent.Username,
            auditEvent.HttpMethod,
            auditEvent.Path,
            auditEvent.StatusCode,
            auditEvent.Success,
            auditEvent.Description,
            auditEvent.IpAddress,
            auditEvent.OccurredAtUtc
        );
    }
}