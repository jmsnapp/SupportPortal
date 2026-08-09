using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ISeveritiesApiClient
{
    Task<IEnumerable<Severity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Severity?> CreateAsync(Severity dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, Severity dto, CancellationToken cancellationToken = default);
}
