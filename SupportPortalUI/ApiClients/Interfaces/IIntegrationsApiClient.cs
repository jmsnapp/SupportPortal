using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IIntegrationsApiClient
{
    Task<IEnumerable<IntegrationDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IntegrationDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
}
