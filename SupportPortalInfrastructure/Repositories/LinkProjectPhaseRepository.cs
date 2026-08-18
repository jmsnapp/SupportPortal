using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalInfrastructure.Repositories;

public class LinkProjectPhaseRepository : GenericRepository<LinkProjectPhaseEntity>
{
    public LinkProjectPhaseRepository(SupportPortalDBContext context) : base(context) { }

    protected override IQueryable<LinkProjectPhaseEntity> WithDetail() =>
        _dbSet.Include(lp => lp.Phase);

    protected override IQueryable<LinkProjectPhaseEntity> WithSummary() =>
        _dbSet.Include(lp => lp.Phase);

}
