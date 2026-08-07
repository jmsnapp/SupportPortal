using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ITicketsApiClient
{
    Task<IEnumerable<TicketDto>> GetActiveAsync(int take = 10, CancellationToken cancellationToken = default);
    Task<TicketDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Int64 id, TicketDto ticket, CancellationToken cancellationToken = default);
}
