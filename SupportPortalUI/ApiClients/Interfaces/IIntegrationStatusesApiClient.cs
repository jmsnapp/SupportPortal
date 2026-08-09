using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IIntegrationStatusesApiClient
{
    Task<IEnumerable<IntegrationStatus>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IntegrationStatus?> CreateAsync(IntegrationStatus dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, IntegrationStatus dto, CancellationToken cancellationToken = default);
}
