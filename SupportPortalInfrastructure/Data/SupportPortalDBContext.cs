using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure.Data;

public class SupportPortalDBContext : DbContext
{
    public SupportPortalDBContext(DbContextOptions<SupportPortalDBContext> options)
        : base(options)
    {
    }

    public DbSet<IndustryEntity> Industries { get; set; } = null!;
    public DbSet<CustomerEntity> Customers { get; set; } = null!;
    public DbSet<ProjectEntity> Projects { get; set; } = null!;
    public DbSet<PhaseEntity> Phases { get; set; } = null!;
    public DbSet<LinkProjectPhaseEntity> LinkProjectPhases { get; set; } = null!;
    public DbSet<IntegrationEntity> Integrations { get; set; } = null!;
    public DbSet<IntegrationStatusEntity> IntegrationStatuses { get; set; } = null!;
    public DbSet<IntegrationTypeEntity> IntegrationTypes { get; set; } = null!;
    public DbSet<IntegrationErrorEntity> IntegrationErrors { get; set; } = null!;
    public DbSet<ProjectNoteEntity> ProjectNotes { get; set; } = null!;
    public DbSet<SeverityEntity> Severities { get; set; } = null!;
    public DbSet<SupportStatusEntity> SupportStatuses { get; set; } = null!;
    public DbSet<TicketEntity> Tickets { get; set; } = null!;
    public DbSet<EscalationEntity> Escalations { get; set; } = null!;
    public DbSet<TicketNoteEntity> TicketNotes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Scaffolded mapping will normally go here.
        // If you later scaffold from the DB, generated mappings will replace or augment these.

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportPortalDBContext).Assembly);

        ConfigureKeySentinel(modelBuilder);

        //ApplySoftDeleteFilters(modelBuilder);

    }

    /// <summary>
    /// These tables seed IDENTITY at 0, so 0 is a real key -- it is the DEFAULT sentinel row
    /// every required foreign key points at. EF's marker for an unset key is default(T), which
    /// here would be that same 0, so PortalEntity's constructor uses -1 instead and the model
    /// has to agree. If the two drift, EF reads the DEFAULT row as a new entity and emits an
    /// explicit identity insert, which SQL Server rejects.
    /// </summary>
    private static void ConfigureKeySentinel(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(PortalEntity).IsAssignableFrom(entityType.ClrType)) continue;

            modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(PortalEntity.Id))
                        .ValueGeneratedOnAdd()
                        .HasSentinel(-1L);

        }

    }

    //private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    //{
    //    // Aggregate roots and child collections only.

    //    modelBuilder.Entity<TicketEntity>().HasQueryFilter(e => !e.Deleted);
    //    modelBuilder.Entity<ProjectEntity>().HasQueryFilter(e => !e.Deleted);
    //    modelBuilder.Entity<TicketNoteEntity>().HasQueryFilter(e => !e.Deleted);
    //    modelBuilder.Entity<ProjectNoteEntity>().HasQueryFilter(e => !e.Deleted);
    //    modelBuilder.Entity<LinkProjectPhaseEntity>().HasQueryFilter(e => !e.Deleted);
    //    modelBuilder.Entity<IntegrationErrorEntity>().HasQueryFilter(e => !e.Deleted);

    //}

}
