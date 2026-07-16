using ResultService.Services;

var builder = WebApplication.CreateBuilder(args);

// Registra los controladores HTTP expuestos por ResultService.
builder.Services.AddControllers();

// Habilita OpenAPI únicamente para documentación y pruebas de desarrollo.
builder.Services.AddOpenApi();

// Registra el cliente HTTP tipado utilizado para consultar los resultados
// agregados que expone VoteService.
builder.Services.AddHttpClient<VoteClient>((serviceProvider, client) =>
{
    var configuration =
        serviceProvider.GetRequiredService<IConfiguration>();

    var voteServiceUrl =
        configuration["Services:VoteService"];

    if (string.IsNullOrWhiteSpace(voteServiceUrl))
    {
        throw new InvalidOperationException(
            "La configuración Services:VoteService es obligatoria."
        );
    }

    if (!Uri.TryCreate(
            voteServiceUrl,
            UriKind.Absolute,
            out var baseAddress))
    {
        throw new InvalidOperationException(
            "Services:VoteService no contiene una URL absoluta válida."
        );
    }

    client.BaseAddress = baseAddress;
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI se expone únicamente en desarrollo para reducir la superficie
    // pública del microservicio.
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "VotoSeguro ResultService",
    status = "Running"
}));

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();