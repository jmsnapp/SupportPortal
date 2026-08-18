using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public abstract class PortalEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : PortalEntity
    {
        protected abstract string TableName { get; }

        protected virtual bool HasUniqueName => true;

        protected virtual int NameLength => 63;
        protected virtual int DescriptionLength => 255;

        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasSentinel(-1L); // -1 means "new"; 0 is now a legitimate existing key

            builder.Property(e => e.Name).HasMaxLength(NameLength).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(DescriptionLength).IsRequired();
            builder.Property(e => e.Deleted).IsRequired();
            builder.Property(e => e.RowVersion).IsRowVersion();

            if (HasUniqueName)
                builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName($"AK_{TableName}_Name");

            builder.HasIndex(e => e.Deleted).HasDatabaseName($"IX_{TableName}_Deleted");

            ConfigureEntity(builder);
        }

        protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);

    }

}
