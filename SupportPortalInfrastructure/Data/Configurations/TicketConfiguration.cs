using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class TicketConfiguration : PortalEntityConfiguration<TicketEntity>
    {
        protected override string TableName => "Tickets";

        protected override int DescriptionLength => 1023;

        protected override void ConfigureEntity(EntityTypeBuilder<TicketEntity> builder)
        {
            builder.Property(e => e.Reproduce).IsRequired();
            builder.Property(e => e.Resolution).IsRequired();
            builder.Property(e => e.ReportedBy).HasMaxLength(63).IsRequired();
            builder.Property(e => e.AssignedTo).HasMaxLength(63).IsRequired();

            // The schema uses DATETIME, not DATETIME2 — say so, or EF assumes datetime2
            // and you get silent precision/range mismatches. See finding #18.
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");
            builder.Property(e => e.ResolutionDate).HasColumnType("datetime");

            builder.HasOne<CustomerEntity>(e => e.Customer)
                   .WithMany()
                   .HasForeignKey(e => e.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<IntegrationEntity>(e => e.Integration)
                   .WithMany()
                   .HasForeignKey(e => e.IntegrationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<SeverityEntity>(e => e.Severity)
                   .WithMany()
                   .HasForeignKey(e => e.SeverityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<SupportStatusEntity>(e => e.Status)
                   .WithMany()
                   .HasForeignKey(e => e.StatusId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<EscalationEntity>(e => e.Escalation)
                   .WithMany()
                   .HasForeignKey(e => e.EscalationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<TicketNoteEntity>(e => e.Notes)
                   .WithOne(n => n.Ticket)
                   .HasForeignKey(n => n.TicketId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.CustomerId).HasDatabaseName("IX_Tickets_Customer");
            builder.HasIndex(e => e.IntegrationId).HasDatabaseName("IX_Tickets_Integration");
            builder.HasIndex(e => e.SeverityId).HasDatabaseName("IX_Tickets_Severity");
            builder.HasIndex(e => e.StatusId).HasDatabaseName("IX_Tickets_Status");
            builder.HasIndex(e => e.EscalationId).HasDatabaseName("IX_Tickets_Escalation");

        }

    }

}
