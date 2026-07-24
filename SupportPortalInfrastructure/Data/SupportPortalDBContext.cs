using Microsoft.EntityFrameworkCore;
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
    }
}