using SupportPortalInfrastructure.Models;
using System.Threading;

namespace SupportPortalInfrastructure.Repositories;

public interface ITicketNoteRepository : IGenericRepository<TicketNote>
{
    Task<IEnumerable<TicketNote>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default);
}