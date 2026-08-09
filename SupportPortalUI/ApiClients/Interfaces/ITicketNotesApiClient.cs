using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ITicketNotesApiClient
{
    Task<IEnumerable<TicketNote>> GetByTicketIdAsync(Int64 ticketId, CancellationToken cancellationToken = default);
    Task<TicketNote?> CreateAsync(TicketNote note, CancellationToken cancellationToken = default);
}
