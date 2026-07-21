using AuditService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Data;

/// <summary>
/// Contexto de persistencia del microservicio de auditoría.
/// </summary>
public sealed class AuditDbContext : DbContext
{
    /// <summary>
    /// Inicializa el contexto de auditoría.
    /// </summary>
    /// <param name="options">
    /// Opciones de conexión y configuración de Entity Framework Core.
    /// </param>
    public AuditDbContext(
        DbContextOptions<AuditDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Conjunto de eventos de auditoría.
    /// </summary>
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    /// <summary>
    /// Configura el modelo relacional de auditoría.
    /// </summary>
    /// <param name="modelBuilder">
    /// Constructor del modelo de Entity Framework Core.
    /// </param>
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.ToTable("AuditEvents", "audit");

            entity.HasKey(item => item.IdAuditEvent);

            entity.Property(item => item.ServiceName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(item => item.Action)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(item => item.EntityType)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(item => item.EntityId)
                .HasMaxLength(100);

            entity.Property(item => item.UserId)
                .HasMaxLength(255);

            entity.Property(item => item.Username)
                .HasMaxLength(100);

            entity.Property(item => item.HttpMethod)
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(item => item.Path)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(item => item.Description)
                .HasMaxLength(1000);

            entity.Property(item => item.IpAddress)
                .HasMaxLength(64);

            entity.Property(item => item.OccurredAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(item => item.OccurredAtUtc);
            entity.HasIndex(item => item.ServiceName);
            entity.HasIndex(item => item.Action);
            entity.HasIndex(item => item.UserId);
        });
    }
}