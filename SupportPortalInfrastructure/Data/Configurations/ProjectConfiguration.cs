using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class ProjectConfiguration : PortalEntityConfiguration<ProjectEntity>
    {
        protected override string TableName => "Projects";

        protected override void ConfigureEntity(EntityTypeBuilder<ProjectEntity> builder)
        {
            // Property name != column name — this is why explicit mapping earns its keep
            builder.Property(e => e.CurrentPhaseId).HasColumnName("CurrentPhase");

            builder.Property(e => e.TargetGoLiveDate).HasColumnType("datetime");
            builder.Property(e => e.ActualGoLiveDate).HasColumnType("datetime");

            // Use the related entity types here (not the FK long properties)
            builder.HasOne<CustomerEntity>(e => e.Customer)
                   .WithMany()
                   .HasForeignKey(e => e.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PhaseEntity>(e => e.CurrentPhase)
                   .WithMany()
                   .HasForeignKey(e => e.CurrentPhaseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<LinkProjectPhaseEntity>(e => e.Phases)
                   .WithOne(pp => pp.Project)
                   .HasForeignKey(p => p.ProjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<ProjectNoteEntity>(e => e.Notes)
                   .WithOne(n => n.Project)
                   .HasForeignKey(n => n.ProjectId)
                   .OnDelete(DeleteBehavior.Restrict);

        }

    }

}
