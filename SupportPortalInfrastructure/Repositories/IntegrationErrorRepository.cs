using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Repositories
{
    public class IntegrationErrorRepository : GenericRepository<IntegrationErrorEntity>
    {
        public IntegrationErrorRepository(SupportPortalDBContext context) : base(context) { }

        // MapIntegrationErrorEntity2IntegrationError maps Integration shallowly,
        // so its children are not needed.
        protected override IQueryable<IntegrationErrorEntity> WithDetail() =>
            _dbSet.Include(e => e.Integration);

        protected override IQueryable<IntegrationErrorEntity> WithSummary() =>
            _dbSet.Include(e => e.Integration);

    }

}
