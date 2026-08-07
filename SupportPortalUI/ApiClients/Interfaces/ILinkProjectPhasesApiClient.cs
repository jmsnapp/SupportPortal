using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ILinkProjectPhasesApiClient
{
    Task<IEnumerable<LinkProjectPhaseDto>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default);
    Task<LinkProjectPhaseDto?> CreateAsync(LinkProjectPhaseDto link, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Int64 id, CancellationToken cancellationToken = default);
}
