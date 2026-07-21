using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
//CORS//
//Controla qué orígenes externos (páginas web) tienen permiso para hacer peticiones a este API Gateway.
//Definimos el nombre de una politica "FrontendCors" 
const string FrontendCors = "FrontendCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, policy =>
    {
        policy
            //Lista blanca de orígenes permitidos para hacer peticiones a este API Gateway.
            .WithOrigins(
                //Entornos de desarrollo locales
                "http://localhost:5173",
                "http://localhost:4173",
                //Url de producción del frontend desplegado en Cloudflare Pages
                "https://sistema-votacion-web-frontend.pages.dev"
            )
            //Permite encabezados personalizados en las peticiones HTTP, 
            // como Authorization para enviar el token JWT.
            .AllowAnyHeader()
            .AllowAnyMethod();
            //Se prohibe el uso de AllowAnyOrigin() para evitar que cualquier página web pueda 
            // hacer peticiones a este API Gateway, lo que sería un riesgo de seguridad.

    });
});

//Autenticacion y Validacion de JWT emitidos por Keycloak. Se utiliza el esquema de autenticación Bearer.
builder.Services //Se define que el mecanismo de autenticación será JWT Bearer, que es el estándar 
                //para tokens de acceso en APIs RESTful.
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //Se configura como se validará el token JWT emitido por Keycloak.
        //Se define la dirección de metadatos de Keycloak, que contiene información sobre el realm,
        // como la clave pública para verificar la firma de los tokens, emisor, JWKS, etc. 
        options.MetadataAddress =
            builder.Configuration["Authentication:MetadataAddress"];

        // Keycloak y el Gateway se comunican mediante HTTP dentro de la red
        // privada de Docker. (permite que consulte la metadata (documento OIDC))
        options.RequireHttpsMetadata = false;

        // Se valida el token JWT emitido por Keycloak según los parámetros configurados.   
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // (Emisor) Verifica que el token provenga exactamente del emisor válido.
            ValidateIssuer = true,
            ValidIssuer =
                builder.Configuration["Authentication:ValidIssuer"],
            // El cliente público actual no requiere validación de audiencia.
            ValidateAudience = false,
            // Impide aceptar tokens expirados.
            ValidateLifetime = true,
            // (Firma) Comprueba criptográficamente que el token JWT fue emitido por Keycloak y no ha sido 
            // alterado.
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

//Autorizacion basada en politicas, crea una regla de seguridad (AdminOnly)
//Exige que el usuario esté autenticado y tenga el rol ADMIN para poder acceder a los endpoints 
// protegidos por esta política.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("ADMIN");
    });
});
//Patron reverse Proxy con YARP.
//Lee desde el archivo de configuracion appsettings.json la sección ReverseProxy, 
// que contiene las rutas y clústeres de los microservicios.
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy")
    );

//Pipeline de middlewares de ASP.NET Core. Se ejecutan en el orden en que se agregan.
//Cada vez que alguien hace una petición a este API Gateway, se ejecutan estos middlewares en el 
// orden definido.
var app = builder.Build();
//Decide a que endpoint para dirigida la peticion (Analiza URL y metodo HTTP para saber que endpoint 
// esta siendo solicitado)
app.UseRouting();
//Valida los permisos de origen cruzado (CORS) para permitir que el frontend pueda hacer peticiones 
// a este API Gateway. (revisa los encabezados de la peticion para ver desde que dominio o pagina web 
// se esta enviando)
app.UseCors(FrontendCors);
//Identifica quien es el usuario descifrando y validando el JWT (Busca en la cabecera Authorization: 
// Bearer ..., valida que la firma sea real, que no haya expirado y que lo haya emitido Keycloak.)
app.UseAuthentication();
//Valida que el usuario tenga permisos para acceder a los recursos solicitados según las politicas de
//  autorizacion definidas (Compara los permisos del paso anterior con los requeridos por el endpoint solicitado)
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