using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPortalInfrastructure.Repositories
{
    public class ProjectRepository : GenericRepository<ProjectEntity>
    {
        public ProjectRepository(SupportPortalDBContext context) : base(context) { }
        public ProjectRepository(SupportPortalDBContext context, Microsoft.Extensions.Options.IOptions<SupportPortalInfrastructure.Configuration.PaginationOptions> options)
            : base(context, options) { }

        protected override IQueryable<ProjectEntity> WithDetail() =>
            _dbSet.AsSplitQuery()
                  .Include(p => p.Customer).ThenInclude(c => c.Industry)
                  .Include(p => p.CurrentPhase)
                  .Include(p => p.Phases).ThenInclude(lp => lp.Phase)
                  .Include(p => p.Notes);

        protected override IQueryable<ProjectEntity> WithSummary() =>
            _dbSet.AsSplitQuery()
                  .Include(p => p.Customer)
                  .Include(p => p.CurrentPhase);

    }

}
