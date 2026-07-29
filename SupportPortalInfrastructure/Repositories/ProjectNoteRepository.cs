using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalInfrastructure.Repositories;

public class ProjectNoteRepository : GenericRepository<ProjectNoteEntity>, IProjectNoteRepository
{

    public ProjectNoteRepository(SupportPortalDBContext context) : base(context) { }

    public async Task<IEnumerable<ProjectNoteEntity>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Where(pn => pn.ProjectId == projectId)
            .ToListAsync(cancellationToken);

}
