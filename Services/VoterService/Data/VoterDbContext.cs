using Microsoft.EntityFrameworkCore;
using VoterService.Models;

namespace VoterService.Data;

/// <summary>
/// Representa la unidad de acceso a datos de VoterService y configura el
/// mapeo entre las entidades del dominio y la base VotoSeguroVoterDb.
/// </summary>
public sealed class VoterDbContext : DbContext
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VoterDbContext"/>.
    /// </summary>
    /// <param name="options">
    /// Opciones de configuración proporcionadas por el contenedor de
    /// dependencias de Entity Framework Core.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce si <paramref name="options"/> es
    /// <see langword="null"/>.
    /// </exception>
    public VoterDbContext(
        DbContextOptions<VoterDbContext> options)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    /// <summary>
    /// Obtiene el conjunto de perfiles electorales administrados por
    /// VoterService.
    /// </summary>
    public DbSet<Votante> Votantes => Set<Votante>();

    /// <summary>
    /// Configura el mapeo, las restricciones y los índices de las entidades
    /// administradas por el contexto.
    /// </summary>
    /// <param name="modelBuilder">
    /// Constructor utilizado por Entity Framework Core para definir el modelo
    /// relacional.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce si <paramref name="modelBuilder"/> es
    /// <see langword="null"/>.
    /// </exception>
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Votante>(entity =>
        {
            entity.ToTable("Votantes", "voter");

            entity.HasKey(voter => voter.IdVotante);

            // Los índices únicos preservan la correspondencia uno a uno entre
            // una identidad de Keycloak y un perfil electoral.
            entity.HasIndex(voter => voter.KeycloakId)
                .IsUnique();

            entity.HasIndex(voter => voter.Cedula)
                .IsUnique();

            entity.HasIndex(voter => voter.CorreoElectronico)
                .IsUnique();

            entity.Property(voter => voter.KeycloakId)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(voter => voter.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(voter => voter.Cedula)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(voter => voter.CorreoElectronico)
                .HasMaxLength(100)
                .IsRequired();

            // SQL Server genera la fecha en UTC cuando el consumidor no envía
            // explícitamente un valor.
            entity.Property(voter => voter.FechaRegistro)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}