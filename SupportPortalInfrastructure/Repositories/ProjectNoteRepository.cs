using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalInfrastructure.Repositories;

public class ProjectNoteRepository : GenericRepository<ProjectNote>, IProjectNoteRepository
{
    public ProjectNoteRepository(SupportPortalDBContext context) : base(context) { }

    public async Task<IEnumerable<ProjectNote>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Where(pn => pn.ProjectId == projectId)
            .ToListAsync(cancellationToken);
}