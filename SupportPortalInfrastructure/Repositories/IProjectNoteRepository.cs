using SupportPortalInfrastructure.Entities;
using System.Threading;

namespace SupportPortalInfrastructure.Repositories;

public interface IProjectNoteRepository : IGenericRepository<ProjectNoteEntity>
{
    Task<IEnumerable<ProjectNoteEntity>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);

}