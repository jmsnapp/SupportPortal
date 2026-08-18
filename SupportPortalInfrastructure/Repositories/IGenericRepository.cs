using System.Linq;
using System.Threading;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure.Repositories;

public interface IGenericRepository<TEntity>
{
    public Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPageAsync(int skip, int take, bool includeDeleted = false, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    IQueryable<TEntity> Query();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}