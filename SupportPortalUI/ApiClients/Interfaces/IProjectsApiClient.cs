using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IProjectsApiClient
{
    Task<IEnumerable<ProjectDto>> GetActiveAsync(int take = 8, CancellationToken cancellationToken = default);
    Task<ProjectDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
}
