using SupportPortalInfrastructure.Entities;
using System.Threading;

namespace SupportPortalInfrastructure.Repositories;

public interface ITicketNoteRepository : IGenericRepository<TicketNoteEntity>
{
    Task<IEnumerable<TicketNoteEntity>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default);
}