using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ISupportStatusesApiClient
{
    Task<IEnumerable<SupportStatus>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SupportStatus?> CreateAsync(SupportStatus dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, SupportStatus dto, CancellationToken cancellationToken = default);
}
