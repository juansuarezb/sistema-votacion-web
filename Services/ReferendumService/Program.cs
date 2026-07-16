using Microsoft.EntityFrameworkCore;
using ReferendumService.Data;
using ReferendumService.Services;

var builder = WebApplication.CreateBuilder(args);

// Registra los controladores responsables de administrar referéndums,
// preguntas, asignaciones y validaciones de elegibilidad.
builder.Services.AddControllers();

// Habilita la generación de OpenAPI para documentación y pruebas locales.
builder.Services.AddOpenApi();

// Registra el contexto de Entity Framework Core utilizando la cadena
// ConnectionStrings:DefaultConnection configurada para ReferendumService.
builder.Services.AddDbContext<ReferendumDbContext>(options =>
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
    // OpenAPI se expone únicamente durante desarrollo para reducir la
    // superficie pública del microservicio en otros entornos.
    app.MapOpenApi();
}

// La base de datos se restaura y configura mediante db-restore.
// No se utiliza EnsureCreated porque podría producir un esquema distinto.
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "VotoSeguro ReferendumService",
    status = "Running"
}));

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();