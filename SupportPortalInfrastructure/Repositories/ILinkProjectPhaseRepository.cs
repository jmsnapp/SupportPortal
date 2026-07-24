using SupportPortalInfrastructure.Entities;
using System.Threading;

namespace SupportPortalInfrastructure.Repositories;

public interface ILinkProjectPhaseRepository : IGenericRepository<LinkProjectPhaseEntity>
{
    Task<IEnumerable<LinkProjectPhaseEntity>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);

}
