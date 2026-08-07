using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients.Interfaces;

public interface ITicketNotesApiClient
{
    Task<IEnumerable<TicketNoteDto>> GetByTicketIdAsync(Int64 ticketId, CancellationToken cancellationToken = default);
    Task<TicketNoteDto?> CreateAsync(TicketNoteDto note, CancellationToken cancellationToken = default);
}
