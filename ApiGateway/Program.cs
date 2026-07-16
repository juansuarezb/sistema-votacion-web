using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCors = "FrontendCors";

// La política restringe el acceso únicamente a los orígenes conocidos del
// frontend, evitando permitir solicitudes arbitrarias mediante AllowAnyOrigin.
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:4173",
                "https://sistema-votacion-web-frontend.pages.dev"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Se utiliza MetadataAddress porque Keycloak se encuentra detrás del
        // API Gateway y el documento OIDC debe resolverse mediante la URL
        // configurada para el entorno actual.
        options.MetadataAddress =
            builder.Configuration["Authentication:MetadataAddress"];

        // Keycloak y el Gateway se comunican mediante HTTP dentro de la red
        // privada de Docker. La exposición pública continúa realizándose por HTTPS.
        // TODO: Habilitar RequireHttpsMetadata en entornos donde la comunicación
        // directa con el proveedor de identidad también utilice HTTPS.
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // El emisor debe coincidir con el realm configurado para evitar
            // aceptar tokens emitidos por otro realm o proveedor.
            ValidateIssuer = true,
            ValidIssuer =
                builder.Configuration["Authentication:ValidIssuer"],

            // El cliente público actual no requiere validación de audiencia.
            // TODO: Configurar y validar una audiencia específica para la API
            // antes de utilizar esta configuración en un entorno productivo.
            ValidateAudience = false,

            // Impide aceptar tokens expirados.
            ValidateLifetime = true,

            // Verifica que la firma del JWT corresponda a una clave publicada
            // por Keycloak.
            ValidateIssuerSigningKey = true,

            // Keycloak utiliza preferred_username como nombre legible del usuario.
            NameClaimType = "preferred_username",

            // ASP.NET Core evaluará los roles mediante ClaimTypes.Role.
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                {
                    return Task.CompletedTask;
                }

                var realmAccessClaim =
                    context.Principal.FindFirst("realm_access");

                if (realmAccessClaim is null ||
                    string.IsNullOrWhiteSpace(realmAccessClaim.Value))
                {
                    return Task.CompletedTask;
                }

                // Keycloak agrupa los roles del realm dentro de un objeto JSON
                // denominado realm_access. Se transforman en ClaimTypes.Role
                // para que las políticas de ASP.NET Core puedan evaluarlos.
                using var document =
                    JsonDocument.Parse(realmAccessClaim.Value);

                if (!document.RootElement.TryGetProperty(
                        "roles",
                        out var rolesElement))
                {
                    return Task.CompletedTask;
                }

                foreach (var roleElement in rolesElement.EnumerateArray())
                {
                    var role = roleElement.GetString();

                    if (!string.IsNullOrWhiteSpace(role))
                    {
                        identity.AddClaim(
                            new Claim(ClaimTypes.Role, role)
                        );
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        // La interfaz puede ocultar operaciones administrativas, pero la
        // autorización definitiva siempre se aplica en el backend.
        policy.RequireAuthenticatedUser();
        policy.RequireRole("ADMIN");
    });
});

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy")
    );

var app = builder.Build();

// El orden de estos middlewares es obligatorio:
// enrutamiento → CORS → autenticación → autorización.
app.UseRouting();
app.UseCors(FrontendCors);
app.UseAuthentication();
app.UseAuthorization();

// Endpoint básico para comprobar que el API Gateway se encuentra operativo.
app.MapGet("/", () => Results.Ok(new
{
    service = "VotoSeguro API Gateway",
    status = "Running"
}))
.RequireCors(FrontendCors);

// YARP aplica las rutas, clústeres y políticas de autorización definidas
// en la sección ReverseProxy de appsettings.json.
app.MapReverseProxy()
    .RequireCors(FrontendCors);

app.Run();