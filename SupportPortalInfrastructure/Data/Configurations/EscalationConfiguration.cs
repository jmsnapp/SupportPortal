using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class EscalationConfiguration : PortalEntityConfiguration<EscalationEntity>
    {
        protected override string TableName => "Escalations";

        protected override void ConfigureEntity(EntityTypeBuilder<EscalationEntity> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

        }

    }

}
