using SupportPortalInfrastructure.Entities;
using System.Threading;

namespace SupportPortalInfrastructure.Repositories;

public interface ILinkProjectPhaseRepository : IGenericRepository<LinkProjectPhaseEntity>
{
    Task<IEnumerable<LinkProjectPhaseEntity>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default);

}
