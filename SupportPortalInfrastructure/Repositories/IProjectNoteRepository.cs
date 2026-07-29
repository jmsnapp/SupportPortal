using SupportPortalInfrastructure.Entities;
using System.Threading;

namespace SupportPortalInfrastructure.Repositories;

public interface IProjectNoteRepository : IGenericRepository<ProjectNoteEntity>
{
    Task<IEnumerable<ProjectNoteEntity>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default);

}