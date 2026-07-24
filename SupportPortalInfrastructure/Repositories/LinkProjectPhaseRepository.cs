using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalInfrastructure.Repositories;

public class LinkProjectPhaseRepository : GenericRepository<LinkProjectPhaseEntity>, ILinkProjectPhaseRepository
{

    public LinkProjectPhaseRepository(SupportPortalDBContext context) : base(context) { }

    public async Task<IEnumerable<LinkProjectPhaseEntity    >> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Where(pn => pn.ProjectId == projectId)
            .ToListAsync(cancellationToken);

}
