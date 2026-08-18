using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace SupportPortalInfrastructure.Data.Configurations
{
    public class CustomerConfiguration : PortalEntityConfiguration<CustomerEntity>
    {
        protected override string TableName => "Customers";

        protected override void ConfigureEntity(EntityTypeBuilder<CustomerEntity> builder)
        {
            builder.Property(e => e.PrimaryContactName).HasMaxLength(63).IsRequired();
            builder.Property(e => e.PrimaryContactEmail).HasMaxLength(63).IsRequired();
            builder.Property(e => e.TechnicalContactName).HasMaxLength(63).IsRequired();
            builder.Property(e => e.TechnicalContactEmail).HasMaxLength(63).IsRequired();

            // The schema uses DATETIME, not DATETIME2 — say so, or EF assumes datetime2
            // and you get silent precision/range mismatches. See finding #18.
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.HasOne<IndustryEntity>(e => e.Industry)
                   .WithMany()
                   .HasForeignKey(e => e.IndustryId)
                   .OnDelete(DeleteBehavior.Restrict);

        }

    }

}
