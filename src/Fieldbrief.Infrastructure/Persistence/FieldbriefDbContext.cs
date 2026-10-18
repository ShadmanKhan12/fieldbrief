using Fieldbrief.Domain.Engagements;
using Microsoft.EntityFrameworkCore;

namespace Fieldbrief.Infrastructure.Persistence;

public class FieldbriefDbContext(DbContextOptions<FieldbriefDbContext> options) : DbContext(options)
{
    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<CustomerSystem> CustomerSystems => Set<CustomerSystem>();
    public DbSet<HealthCheckDefinition> HealthChecks => Set<HealthCheckDefinition>();
    public DbSet<HealthCheckResult> HealthCheckResults => Set<HealthCheckResult>();
    public DbSet<FieldDecision> Decisions => Set<FieldDecision>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<FieldNote> Notes => Set<FieldNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Engagement>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(40).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Title).HasMaxLength(300).IsRequired();
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
            e.Property(x => x.Owner).HasMaxLength(120);
        });

        modelBuilder.Entity<CustomerSystem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Kind).HasMaxLength(40);
            e.Property(x => x.BaseUrl).HasMaxLength(2000);
            e.HasOne(x => x.Engagement).WithMany(x => x.Systems).HasForeignKey(x => x.EngagementId);
        });

        modelBuilder.Entity<HealthCheckDefinition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Method).HasMaxLength(16);
            e.Property(x => x.Path).HasMaxLength(500);
            e.HasOne(x => x.CustomerSystem).WithMany(x => x.HealthChecks).HasForeignKey(x => x.CustomerSystemId);
        });

        modelBuilder.Entity<HealthCheckResult>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Error).HasMaxLength(2000);
            e.HasOne(x => x.HealthCheckDefinition).WithMany(x => x.Results).HasForeignKey(x => x.HealthCheckDefinitionId);
            e.HasIndex(x => new { x.HealthCheckDefinitionId, x.RanAt });
        });

        modelBuilder.Entity<FieldDecision>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(300).IsRequired();
            e.Property(x => x.MadeBy).HasMaxLength(120);
            e.HasOne(x => x.Engagement).WithMany(x => x.Decisions).HasForeignKey(x => x.EngagementId);
        });

        modelBuilder.Entity<ChecklistItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(300).IsRequired();
            e.Property(x => x.Phase).HasMaxLength(40);
            e.HasOne(x => x.Engagement).WithMany(x => x.Checklist).HasForeignKey(x => x.EngagementId);
        });

        modelBuilder.Entity<FieldNote>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Author).HasMaxLength(120);
            e.HasOne(x => x.Engagement).WithMany(x => x.Notes).HasForeignKey(x => x.EngagementId);
        });
    }
}
