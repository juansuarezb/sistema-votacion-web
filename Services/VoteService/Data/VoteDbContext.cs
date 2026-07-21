using Microsoft.EntityFrameworkCore;
using VoteService.Models;

namespace VoteService.Data;

/// <summary>
/// Representa la unidad de acceso a datos de VoteService y configura el
/// mapeo de los votos almacenados en VotoSeguroVoteDb.
/// </summary>
public sealed class VoteDbContext : DbContext
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VoteDbContext"/>.
    /// </summary>
    /// <param name="options">
    /// Opciones de configuración proporcionadas por Entity Framework Core.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Se produce si <paramref name="options"/> es
    /// <see langword="null"/>.
    /// </exception>
    public VoteDbContext(
        DbContextOptions<VoteDbContext> options)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    /// <summary>
    /// Obtiene el conjunto de votos registrados.
    /// </summary>
    public DbSet<Vote> Votes => Set<Vote>();

    /// <summary>
    /// Configura la tabla, clave, índices y propiedades de la entidad
    /// <see cref="Vote"/>.
    /// </summary>
    /// <param name="modelBuilder">
    /// Constructor del modelo relacional.
    /// </param>
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.ToTable("Votes", "vote");

            entity.HasKey(vote => vote.IdVote);

            entity.HasIndex(vote => vote.IdReferendum);

            entity.HasIndex(vote => vote.IdQuestion);

            entity.Property(vote => vote.TipoVoto)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(vote => vote.Fecha)
                .HasDefaultValueSql("GETUTCDATE()");

            // TODO: Incorporar una restricción CHECK en la base de datos para
            // limitar TipoVoto a SI, NO, BLANCO o NULO.
        });
    }
}