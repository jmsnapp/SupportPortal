using SupportPortalInfrastructure.Entities;
using System.Threading;

namespace SupportPortalInfrastructure.Repositories;

public interface ITicketNoteRepository : IGenericRepository<TicketNoteEntity>
{
    Task<IEnumerable<TicketNoteEntity>> GetByTicketIdAsync(Int64 ticketId, CancellationToken cancellationToken = default);
}