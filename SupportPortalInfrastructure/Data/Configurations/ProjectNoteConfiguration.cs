using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class ProjectNoteConfiguration : PortalEntityConfiguration<ProjectNoteEntity>
    {
        protected override string TableName => "ProjectNotes";

        protected override void ConfigureEntity(EntityTypeBuilder<ProjectNoteEntity> builder)
        {
            builder.Property(e => e.CreateTime).HasColumnType("datetime");

        }

    }

}
