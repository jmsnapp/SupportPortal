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

    public async Task<IEnumerable<LinkProjectPhaseEntity>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .FromSql($"SELECT * FROM LinkProjectPhase WHERE Deleted = 0 AND ProjectId = {projectId}") 
            .ToListAsync(cancellationToken);

}
