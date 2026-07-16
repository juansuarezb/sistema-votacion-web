using System.Net.Http.Json;
using VoterService.Models;

namespace VoterService.Services;

/// <summary>
/// Encapsula la comunicación HTTP entre VoterService y AuditService.
/// </summary>
/// <remarks>
/// La auditoría es una operación secundaria. Un fallo de AuditService no debe
/// invalidar una actualización que ya fue persistida correctamente.
/// </remarks>
public sealed class AuditClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditClient> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AuditClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// Cliente HTTP configurado con la dirección interna de AuditService.
    /// </param>
    /// <param name="logger">
    /// Servicio utilizado para registrar errores de comunicación.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando alguna dependencia es
    /// <see langword="null"/>.
    /// </exception>
    public AuditClient(
        HttpClient httpClient,
        ILogger<AuditClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(logger);

        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Intenta registrar un evento en AuditService sin interrumpir la
    /// operación principal cuando el servicio de auditoría no está disponible.
    /// </summary>
    /// <param name="request">
    /// Evento que debe almacenarse.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación.
    /// </param>
    /// <returns>
    /// <see langword="true"/> cuando AuditService acepta el evento; de lo
    /// contrario, <see langword="false"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Se produce cuando la solicitud original fue cancelada.
    /// </exception>
    public async Task<bool> TryWriteAsync(
        CreateAuditEventRequest request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "/api/audit/events",
                request,
                ct
            );

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var responseBody =
                await response.Content.ReadAsStringAsync(ct);

            _logger.LogWarning(
                "AuditService rechazó el evento {Action}. HTTP {StatusCode}. Respuesta: {ResponseBody}",
                request.Action,
                (int)response.StatusCode,
                responseBody
            );

            return false;
        }
        catch (OperationCanceledException)
            when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // La auditoría no debe deshacer una actualización que ya fue
            // persistida correctamente en VoterService.
            _logger.LogWarning(
                ex,
                "No se pudo registrar el evento {Action} en AuditService.",
                request.Action
            );

            return false;
        }
    }
}