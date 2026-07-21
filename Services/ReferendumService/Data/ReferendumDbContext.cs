using Microsoft.EntityFrameworkCore;
using ReferendumService.Models;

namespace ReferendumService.Data;

/// <summary>
/// Representa la unidad de acceso a datos de ReferendumService y configura
/// el mapeo relacional de referéndums, preguntas y asignaciones.
/// </summary>
public sealed class ReferendumDbContext : DbContext
{
    /// <summary>
    /// Inicializa una nueva instancia de
    /// <see cref="ReferendumDbContext"/>.
    /// </summary>
    /// <param name="options">
    /// Opciones proporcionadas por Entity Framework Core.
    /// </param>
    public ReferendumDbContext(
        DbContextOptions<ReferendumDbContext> options)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    /// <summary>
    /// Obtiene el conjunto de referéndums.
    /// </summary>
    public DbSet<Referendum> Referendums => Set<Referendum>();

    /// <summary>
    /// Obtiene el conjunto de preguntas de referéndum.
    /// </summary>
    public DbSet<ReferendumQuestion> ReferendumQuestions =>
        Set<ReferendumQuestion>();

    /// <summary>
    /// Obtiene el conjunto de asignaciones entre preguntas y votantes.
    /// </summary>
    public DbSet<ReferendumQuestionVoter> ReferendumQuestionVoters =>
        Set<ReferendumQuestionVoter>();

    /// <summary>
    /// Configura las tablas, claves, restricciones, índices y relaciones
    /// administradas por el contexto.
    /// </summary>
    /// <param name="modelBuilder">
    /// Constructor del modelo relacional.
    /// </param>
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Referendum>(entity =>
        {
            entity.ToTable("Referendums", "referendum");

            entity.HasKey(item => item.IdReferendum);

            entity.Property(item => item.Titulo)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(item => item.Descripcion)
                .HasMaxLength(1000);

            entity.Property(item => item.Estado)
                .HasMaxLength(30)
                .HasDefaultValue("BORRADOR")
                .IsRequired();

            entity.Property(item => item.FechaCreacion)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(item => item.Estado);

            entity.HasIndex(item => new
            {
                item.FechaInicio,
                item.FechaCierre
            });
        });

        modelBuilder.Entity<ReferendumQuestion>(entity =>
        {
            entity.ToTable(
                "ReferendumQuestions",
                "referendum"
            );

            entity.HasKey(question => question.IdQuestion);

            entity.Property(question => question.Texto)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasOne(question => question.Referendum)
                .WithMany(referendum => referendum.Preguntas)
                .HasForeignKey(question => question.IdReferendum)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReferendumQuestionVoter>(entity =>
        {
            entity.ToTable(
                "ReferendumQuestionVoters",
                "referendum"
            );

            entity.HasKey(assignment => new
            {
                assignment.IdReferendum,
                assignment.IdQuestion,
                assignment.IdVotante
            });

            entity.Property(assignment => assignment.HaVotado)
                .HasDefaultValue(false);

            entity.Property(assignment => assignment.FechaAsignacion)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(assignment => assignment.IdVotante);

            entity.HasOne(assignment => assignment.Referendum)
                .WithMany()
                .HasForeignKey(assignment =>
                    assignment.IdReferendum)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(assignment => assignment.Question)
                .WithMany()
                .HasForeignKey(assignment =>
                    assignment.IdQuestion)
                .OnDelete(DeleteBehavior.NoAction);

            // FIXME: La relación con Question usa únicamente IdQuestion.
            // La base debería impedir explícitamente que IdQuestion pertenezca
            // a un referéndum diferente de IdReferendum.
        });
    }
}