using Microsoft.EntityFrameworkCore;
using VoterService.Data;
using VoterService.Services;

var builder = WebApplication.CreateBuilder(args);

// Registra los controladores que exponen las operaciones CRUD de los perfiles
// electorales gestionados por VoterService.
builder.Services.AddControllers();

// Habilita OpenAPI para documentación y pruebas durante el desarrollo.
builder.Services.AddOpenApi();

// Registra el contexto de Entity Framework Core utilizando la conexión
// exclusiva de VoterService.
builder.Services.AddDbContext<VoterDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString(
            "DefaultConnection"
        );

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "ConnectionStrings:DefaultConnection no está configurado."
        );
    }

    options.UseSqlServer(connectionString);
});

// Registra el cliente encargado de enviar eventos funcionales hacia
// AuditService dentro de la red privada de Docker.
builder.Services.AddHttpClient<AuditClient>(client =>
{
    var auditServiceUrl =
        builder.Configuration["Services:AuditService"];

    if (string.IsNullOrWhiteSpace(auditServiceUrl))
    {
        throw new InvalidOperationException(
            "Services:AuditService no está configurado."
        );
    }

    if (!Uri.TryCreate(
            auditServiceUrl,
            UriKind.Absolute,
            out var auditBaseAddress))
    {
        throw new InvalidOperationException(
            "Services:AuditService no contiene una URL absoluta válida."
        );
    }

    client.BaseAddress = auditBaseAddress;
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI se publica únicamente durante desarrollo para reducir la
    // superficie expuesta en otros entornos.
    app.MapOpenApi();
}

// VoterService confía actualmente en la autorización aplicada por el Gateway.
// FIXME: Configurar autenticación y autorización internas como defensa en
// profundidad antes de utilizar este diseño en producción.
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "VotoSeguro VoterService",
    status = "Running"
}));

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();