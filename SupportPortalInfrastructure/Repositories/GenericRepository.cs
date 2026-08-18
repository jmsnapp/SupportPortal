using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalInfrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : PortalEntity
{
    protected readonly SupportPortalDBContext _context;

    protected readonly DbSet<TEntity> _dbSet;

    public const int MaxPageSize = 200;

    public GenericRepository(SupportPortalDBContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TEntity>();
    }

    /// <summary>
    /// Every read goes through this query. The base implementation returns the entity's
    /// own columns only; repositories whose entity has navigation properties override it
    /// to add the Include chain that entity's mapper depends on.
    /// Do not add AsNoTracking() here — GetByIdAsync feeds the update path.
    /// </summary>
    protected virtual IQueryable<TEntity> WithDetail() => _dbSet;

    /// <summary>List/dropdown shape: own columns only. Override only where a list
    /// genuinely needs a navigation (e.g. Customer.Industry for a grid column).</summary>
    protected virtual IQueryable<TEntity> WithSummary() => _dbSet;

    public async Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPageAsync(int skip, int take, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> scope = includeDeleted
            ? _dbSet.IgnoreQueryFilters()
            : _dbSet.Where(x => !x.Deleted);

        int total = await scope.CountAsync(cancellationToken);

        IQueryable<TEntity> detail = includeDeleted
            ? WithSummary().IgnoreQueryFilters()
            : WithSummary().Where(x => !x.Deleted);

        List<TEntity> items = await detail
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip(Math.Max(skip, 0))
            .Take(Math.Clamp(take, 1, MaxPageSize))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    // Tracked, and filters ignored: Update() maps onto the instance this returns, and
    // since update *is* delete in this design, a soft-deleted row must stay fetchable
    // so it can be restored.
    public async Task<TEntity?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default) =>
        await WithDetail()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<TEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await WithDetail()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && !x.Deleted, cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(TEntity entity)
    {
        // If this key is already tracked (e.g. the controller's existence check loaded it),
        // copy onto the tracked instance instead of attaching a second one.
        var tracked = _context.ChangeTracker
                              .Entries<TEntity>()
                              .FirstOrDefault(e => e.Entity.Id == entity.Id);

        if (tracked is not null)
        {
            tracked.CurrentValues.SetValues(entity);
            return;
        }

        _context.Entry(entity).State = EntityState.Modified;

    }

    public IQueryable<TEntity> Query() => _dbSet.AsQueryable();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

}
