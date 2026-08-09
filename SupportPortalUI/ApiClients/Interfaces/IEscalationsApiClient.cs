using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface IEscalationsApiClient
{
    Task<IEnumerable<Escalation>> GetActiveAsync(int take = 5, CancellationToken cancellationToken = default);
    Task<Escalation?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
    Task<Escalation?> CreateAsync(Escalation dto, CancellationToken cancellationToken = default);
}
