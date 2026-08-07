using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalInfrastructure.Repositories;

public class TicketNoteRepository : GenericRepository<TicketNoteEntity>, ITicketNoteRepository
{
    public TicketNoteRepository(SupportPortalDBContext context) : base(context) { }

    public async Task<IEnumerable<TicketNoteEntity >> GetByTicketIdAsync(Int64 ticketId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .FromSql($"SELECT * FROM TicketNote WHERE Deleted = 0 AND TicketId = {ticketId}")
            .ToListAsync(cancellationToken);

}
