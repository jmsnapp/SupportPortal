using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalInfrastructure.Repositories;

public class TicketRepository : GenericRepository<TicketEntity>
{
    public TicketRepository(SupportPortalDBContext context) : base(context) { }
    public TicketRepository(SupportPortalDBContext context, Microsoft.Extensions.Options.IOptions<SupportPortalInfrastructure.Configuration.PaginationOptions> options)
        : base(context, options) { }

    protected override IQueryable<TicketEntity> WithDetail() =>
        _dbSet.AsSplitQuery()
              .Include(t => t.Customer).ThenInclude(c => c.Industry)
              .Include(t => t.Integration).ThenInclude(i => i.IntegrationType)
              .Include(t => t.Integration).ThenInclude(i => i.CurrentStatus)
              .Include(t => t.Integration).ThenInclude(i => i.Customer).ThenInclude(c => c.Industry)
              .Include(t => t.Severity)
              .Include(t => t.Status)
              .Include(t => t.Escalation)
              .Include(t => t.Notes);

    protected override IQueryable<TicketEntity> WithSummary() =>
        _dbSet.AsSplitQuery()
              .Include(t => t.Customer)
              .Include(t => t.Integration)
              .Include(t => t.Severity)
              .Include(t => t.Status);

}
