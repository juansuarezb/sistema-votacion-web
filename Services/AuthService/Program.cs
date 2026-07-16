using AuthService.Options;
using AuthService.Services;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Registra los controladores HTTP del microservicio.
builder.Services.AddControllers();

// Habilita OpenAPI para desarrollo.
builder.Services.AddOpenApi();

// Permite devolver respuestas ProblemDetails para errores no controlados.
builder.Services.AddProblemDetails();

// Vincula y valida la configuración de Keycloak al iniciar la aplicación.
builder.Services
    .AddOptions<KeycloakOptions>()
    .Bind(builder.Configuration.GetSection("Keycloak"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Vincula y valida las URLs de los servicios dependientes.
builder.Services
    .AddOptions<ServiceOptions>()
    .Bind(builder.Configuration.GetSection("Services"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Registra los clientes HTTP utilizados para administrar Keycloak y
// comunicarse con VoterService.
builder.Services.AddHttpClient<KeycloakAdminClient>();
builder.Services.AddHttpClient<VoterServiceClient>();

// Registra el cliente HTTP utilizado para enviar eventos funcionales al
// microservicio de auditoría.
builder.Services.AddHttpClient<AuditClient>(client =>
{
    var auditServiceUrl =
        builder.Configuration["Services:AuditServiceUrl"];

    if (string.IsNullOrWhiteSpace(auditServiceUrl))
    {
        throw new InvalidOperationException(
            "Services:AuditServiceUrl no está configurado."
        );
    }

    if (!Uri.TryCreate(
            auditServiceUrl,
            UriKind.Absolute,
            out var auditBaseAddress))
    {
        throw new InvalidOperationException(
            "Services:AuditServiceUrl no contiene una URL absoluta válida."
        );
    }

    client.BaseAddress = auditBaseAddress;
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler(errorApplication =>
{
    errorApplication.Run(async context =>
    {
        var exceptionFeature =
            context.Features.Get<IExceptionHandlerFeature>();

        if (exceptionFeature?.Error is not null)
        {
            app.Logger.LogError(
                exceptionFeature.Error,
                "Se produjo una excepción no controlada en AuthService."
            );
        }

        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Error interno del servidor",
            detail: "No se pudo completar la operación solicitada."
        ).ExecuteAsync(context);
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new
{
    service = "VotoSeguro AuthService",
    status = "Running"
}));

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();