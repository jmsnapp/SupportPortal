using Microsoft.Extensions.Options;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Configuration;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class TicketsController : GenericController<Ticket, TicketListItem, TicketEntity>
    {
        public TicketsController(IGenericRepository<Ticket, TicketListItem, TicketEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options) { }

        protected override void MapModelToEntity(Ticket model, TicketEntity entity) =>
            DBMapper.MapTicket2TicketEntity(model, ref entity);

    }

}
