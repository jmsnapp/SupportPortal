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

    public GenericRepository(SupportPortalDBContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.ToListAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.Where(x => x.Deleted == false).ToListAsync(cancellationToken);

    public async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default) =>
        await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<TEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        TEntity objReturn = null;

        List<TEntity> lstTemp = await _dbSet.Where(x => x.Name == name && x.Deleted == false).ToListAsync(cancellationToken);
        if(lstTemp.Count > 0)
           objReturn = lstTemp.First();

        return objReturn;

    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public IQueryable<TEntity> Query() => _dbSet.AsQueryable();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

}
