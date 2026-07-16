using Microsoft.EntityFrameworkCore;
using VoteService.Data;
using VoteService.Services;

var builder = WebApplication.CreateBuilder(args);

// Registra los controladores responsables de emitir, consultar y resumir votos.
builder.Services.AddControllers();

// Habilita OpenAPI para documentación y pruebas durante el desarrollo.
builder.Services.AddOpenApi();

// Registra el contexto de Entity Framework Core utilizando la cadena de
// conexión exclusiva de VoteService.
builder.Services.AddDbContext<VoteDbContext>(options =>
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

// Registra el cliente HTTP tipado que consulta ReferendumService para validar
// la elegibilidad y actualizar el estado de participación del votante.
builder.Services.AddHttpClient<ReferendumClient>(client =>
{
    var referendumServiceUrl =
        builder.Configuration[
            "Services:ReferendumService"
        ];

    if (string.IsNullOrWhiteSpace(referendumServiceUrl))
    {
        throw new InvalidOperationException(
            "Services:ReferendumService no está configurado."
        );
    }

    if (!Uri.TryCreate(
            referendumServiceUrl,
            UriKind.Absolute,
            out var referendumBaseAddress))
    {
        throw new InvalidOperationException(
            "Services:ReferendumService no contiene una URL válida."
        );
    }

    client.BaseAddress = referendumBaseAddress;
    client.Timeout = TimeSpan.FromSeconds(15);
});

// Registra el cliente HTTP encargado de enviar eventos funcionales hacia
// AuditService dentro de la red privada de Docker.
builder.Services.AddHttpClient<AuditClient>(client =>
{
    var auditServiceUrl =
        builder.Configuration[
            "Services:AuditService"
        ];

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
            "Services:AuditService no contiene una URL válida."
        );
    }

    client.BaseAddress = auditBaseAddress;
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI se expone únicamente en desarrollo para reducir la superficie
    // pública del microservicio.
    app.MapOpenApi();
}

// La base de datos se restaura mediante scripts y respaldos oficiales.
// No se utiliza EnsureCreated porque podría generar un esquema diferente.
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "VotoSeguro VoteService",
    status = "Running"
}));

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();