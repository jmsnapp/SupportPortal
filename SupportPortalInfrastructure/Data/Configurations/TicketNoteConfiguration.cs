using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class TicketNoteConfiguration : PortalEntityConfiguration<TicketNoteEntity>
    {
        protected override string TableName => "TicketNotes";

        protected override void ConfigureEntity(EntityTypeBuilder<TicketNoteEntity> builder)
        {
            builder.Property(e => e.CreateTime).HasColumnType("datetime");

        }

    }

}
