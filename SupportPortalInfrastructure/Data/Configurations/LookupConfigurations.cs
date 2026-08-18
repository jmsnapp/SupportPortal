using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Data.Configurations
{
    public class IndustryConfiguration : PortalEntityConfiguration<IndustryEntity>
    {
        protected override string TableName => "Industries";

        protected override void ConfigureEntity(EntityTypeBuilder<IndustryEntity> builder) { }

    }

    public class SeverityConfiguration : PortalEntityConfiguration<SeverityEntity>
    {
        protected override string TableName => "Severities";

        protected override void ConfigureEntity(EntityTypeBuilder<SeverityEntity> builder) { }

    }

    public class PhaseConfiguration : PortalEntityConfiguration<PhaseEntity>
    {
        protected override string TableName => "Phases";

        protected override void ConfigureEntity(EntityTypeBuilder<PhaseEntity> builder) { }

    }

    public class IntegrationTypeConfiguration : PortalEntityConfiguration<IntegrationTypeEntity>
    {
        protected override string TableName => "IntegrationTypes";

        protected override void ConfigureEntity(EntityTypeBuilder<IntegrationTypeEntity> builder) { }

    }

    public class IntegrationStatusConfiguration : PortalEntityConfiguration<IntegrationStatusEntity>
    {
        protected override string TableName => "IntegrationStatuses";

        protected override void ConfigureEntity(EntityTypeBuilder<IntegrationStatusEntity> builder) { }

    }

    public class SupportStatusConfiguration : PortalEntityConfiguration<SupportStatusEntity>
    {
        protected override string TableName => "SupportStatuses";

        protected override void ConfigureEntity(EntityTypeBuilder<SupportStatusEntity> builder) { }

    }

}
