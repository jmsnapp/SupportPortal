using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Repositories
{
    public class IntegrationRepository : GenericRepository<IntegrationEntity>
    {
        public IntegrationRepository(SupportPortalDBContext context) : base(context) { }
        public IntegrationRepository(SupportPortalDBContext context, Microsoft.Extensions.Options.IOptions<SupportPortalInfrastructure.Configuration.PaginationOptions> options)
            : base(context, options) { }

        protected override IQueryable<IntegrationEntity> WithDetail() =>
            _dbSet.Include(i => i.Customer).ThenInclude(c => c.Industry)
                  .Include(i => i.IntegrationType)
                  .Include(i => i.CurrentStatus);

        protected override IQueryable<IntegrationEntity> WithSummary() =>
            _dbSet.Include(i => i.Customer)
                  .Include(i => i.IntegrationType)
                  .Include(i => i.CurrentStatus);

    }

}
