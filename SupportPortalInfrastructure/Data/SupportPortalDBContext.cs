using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Models;

namespace SupportPortalInfrastructure.Data;

public class SupportPortalDBContext : DbContext
{
    public SupportPortalDBContext(DbContextOptions<SupportPortalDBContext> options)
        : base(options)
    {
    }

    public DbSet<Industry> Industries { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Phase> Phases { get; set; } = null!;
    public DbSet<LinkProjectPhase> LinkProjectPhases { get; set; } = null!;
    public DbSet<Integration> Integrations { get; set; } = null!;
    public DbSet<IntegrationStatus> IntegrationStatuses { get; set; } = null!;
    public DbSet<IntegrationType> IntegrationTypes { get; set; } = null!;
    public DbSet<IntegrationError> IntegrationErrors { get; set; } = null!;
    public DbSet<ProjectNote> ProjectNotes { get; set; } = null!;
    public DbSet<Severity> Severities { get; set; } = null!;
    public DbSet<SupportStatus> SupportStatuses { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;
    public DbSet<Escalation> Escalations { get; set; } = null!;
    public DbSet<TicketNote> TicketNotes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Scaffolded mapping will normally go here.
        // If you later scaffold from the DB, generated mappings will replace or augment these.
    }
}