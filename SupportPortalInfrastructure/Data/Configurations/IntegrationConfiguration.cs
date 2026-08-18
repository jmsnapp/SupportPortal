using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class IntegrationConfiguration : PortalEntityConfiguration<IntegrationEntity>
    {
        protected override string TableName => "Integrations";

        protected override int DescriptionLength => 127;

        protected override void ConfigureEntity(EntityTypeBuilder<IntegrationEntity> builder) 
        {
            builder.Property(e => e.LastFailedSync).HasColumnType("datetime");
            builder.Property(e => e.LastSuccessfulSync).HasColumnType("datetime");

            // Use the related entity types here (not the FK long properties)
            builder.HasOne<CustomerEntity>(e => e.Customer)
                   .WithMany()
                   .HasForeignKey(e => e.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<IntegrationTypeEntity>(e => e.IntegrationType)
                   .WithMany()
                   .HasForeignKey(e => e.IntegrationTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<IntegrationStatusEntity>(e => e.CurrentStatus)
                   .WithMany()
                   .HasForeignKey(e => e.CurrentStatusId)
                   .OnDelete(DeleteBehavior.Restrict);

        }

    }

}
