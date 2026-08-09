using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ILinkProjectPhasesApiClient
{
    Task<IEnumerable<ProjectPhase>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default);
    Task<ProjectPhase?> CreateAsync(ProjectPhase link, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Int64 id, CancellationToken cancellationToken = default);
}
