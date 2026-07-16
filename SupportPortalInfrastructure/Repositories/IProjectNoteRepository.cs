using SupportPortalInfrastructure.Models;
using System.Threading;

namespace SupportPortalInfrastructure.Repositories;

public interface IProjectNoteRepository : IGenericRepository<ProjectNote>
{
    Task<IEnumerable<ProjectNote>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
}