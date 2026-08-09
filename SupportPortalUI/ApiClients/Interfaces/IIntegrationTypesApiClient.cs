using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IIntegrationTypesApiClient
{
    Task<IEnumerable<IntegrationType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IntegrationType?> CreateAsync(IntegrationType dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, IntegrationType dto, CancellationToken cancellationToken = default);
}
