using AuthService.Options;
using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<KeycloakOptions>(
    builder.Configuration.GetSection("Keycloak")
);

builder.Services.Configure<ServiceOptions>(
    builder.Configuration.GetSection("Services")
);

builder.Services.AddHttpClient<KeycloakAdminClient>();
builder.Services.AddHttpClient<VoterServiceClient>();

builder.Services.AddHealthChecks();

var app = builder.Build();

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