using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalInfrastructure.Repositories;

public class TicketNoteRepository : GenericRepository<TicketNote>, ITicketNoteRepository
{
    public TicketNoteRepository(SupportPortalDBContext context) : base(context) { }

    public async Task<IEnumerable<TicketNote>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Where(tn => tn.TicketId == ticketId)
            .ToListAsync(cancellationToken);
}