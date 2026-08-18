using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class LinkProjectPhaseConfiguration : PortalEntityConfiguration<LinkProjectPhaseEntity>
    {
        protected override string TableName => "LinkProjectPhases";

        protected override void ConfigureEntity(EntityTypeBuilder<LinkProjectPhaseEntity> builder)
        {
            // Without this EF defaults to decimal(18,2).
            builder.Property(e => e.Percentage).HasPrecision(5, 2);

            builder.Property(e => e.Order).HasColumnName("Order");

            builder.HasIndex(e => new { e.ProjectId, e.PhaseId })
                   .IsUnique()
                   .HasDatabaseName("AK_LinkProjectPhase_ProjectPhase");

            builder.HasOne<PhaseEntity>(e => e.Phase)
                   .WithMany()
                   .HasForeignKey(e => e.PhaseId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Project side configured from ProjectConfiguration

        }

    }

}
