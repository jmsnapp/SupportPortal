using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IProjectsApiClient
{
    Task<IEnumerable<Project>> GetActiveAsync(int take = 8, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
}
