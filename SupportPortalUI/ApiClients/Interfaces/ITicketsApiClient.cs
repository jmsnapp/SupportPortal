using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ITicketsApiClient
{
    Task<IEnumerable<Ticket>> GetActiveAsync(int take = 10, CancellationToken cancellationToken = default);

    Task<Ticket?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Int64 id, Ticket ticket, CancellationToken cancellationToken = default);

}
