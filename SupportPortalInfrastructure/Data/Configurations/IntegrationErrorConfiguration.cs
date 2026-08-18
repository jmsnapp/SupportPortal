using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class IntegrationErrorConfiguration : PortalEntityConfiguration<IntegrationErrorEntity>
    {
        protected override string TableName => "IntegrationErrors";

        protected override void ConfigureEntity(EntityTypeBuilder<IntegrationErrorEntity> builder)
        {
            builder.Property(e => e.ErrorTime).HasColumnType("datetime");

            // Use the related entity types here (not the FK long properties)
            builder.HasOne<IntegrationEntity>(e => e.Integration)
                   .WithMany()
                   .HasForeignKey(e => e.IntegrationId)
                   .OnDelete(DeleteBehavior.Restrict);

        }

    }

}
