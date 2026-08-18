using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class TicketsController : GenericController<TicketEntity, Ticket>
    {
        public TicketsController(IGenericRepository<TicketEntity> repo) : base(repo) { }

        protected override Ticket MapEntityToModel(TicketEntity entity)
        {
            var model = DBMapper.MapTicketEntity2Ticket(entity);
            return model;

        }

        protected override void MapModelToEntity(Ticket model, TicketEntity entity) =>
            DBMapper.MapTicket2TicketEntity(model, ref entity);

    }

}
