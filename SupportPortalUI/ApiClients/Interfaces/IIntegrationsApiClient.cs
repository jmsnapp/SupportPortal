using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IIntegrationsApiClient
{
    Task<IEnumerable<Integration>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Integration?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
}
