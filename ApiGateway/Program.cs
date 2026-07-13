using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCors = "FrontendCors";

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
        options.MetadataAddress =
            builder.Configuration["Authentication:MetadataAddress"];

        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer =
                builder.Configuration["Authentication:ValidIssuer"],

            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            NameClaimType = "preferred_username",
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

                if (
                    realmAccessClaim is null ||
                    string.IsNullOrWhiteSpace(realmAccessClaim.Value)
                )
                {
                    return Task.CompletedTask;
                }

                using var document =
                    JsonDocument.Parse(realmAccessClaim.Value);

                if (
                    !document.RootElement.TryGetProperty(
                        "roles",
                        out var rolesElement
                    )
                )
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

app.UseRouting();
app.UseCors(FrontendCors);
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "VotoSeguro API Gateway",
    status = "Running"
}))
.RequireCors(FrontendCors);

app.MapReverseProxy()
    .RequireCors(FrontendCors);

app.Run();