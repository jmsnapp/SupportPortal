using System.Linq;
using System.Threading;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure.Repositories;

public interface IGenericRepository<PortalEntity>
{
    Task<IEnumerable<PortalEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<PortalEntity>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    Task<PortalEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    Task AddAsync(PortalEntity entity, CancellationToken cancellationToken = default);

    void Update(PortalEntity entity);

    void Remove(PortalEntity entity);

    IQueryable<PortalEntity> Query();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}