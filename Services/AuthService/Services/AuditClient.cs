using System.Net.Http.Json;
using AuthService.Models;

namespace AuthService.Services;

/// <summary>
/// Encapsula el envío de eventos funcionales desde AuthService hacia
/// AuditService.
/// </summary>
/// <remarks>
/// La auditoría se considera una operación secundaria. La indisponibilidad de
/// AuditService no debe invalidar una operación de identidad completada.
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
    /// Servicio utilizado para registrar fallos de auditoría.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce cuando una dependencia es <see langword="null"/>.
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
    /// Intenta registrar un evento en AuditService.
    /// </summary>
    /// <param name="request">
    /// Evento funcional que debe almacenarse.
    /// </param>
    /// <param name="ct">
    /// Token utilizado para cancelar la operación.
    /// </param>
    /// <returns>
    /// <see langword="true"/> si AuditService aceptó el evento; de lo
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
            _logger.LogWarning(
                ex,
                "No se pudo registrar el evento {Action} en AuditService.",
                request.Action
            );

            return false;
        }
    }
}