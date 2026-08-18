using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class TicketNotesController : GenericController<TicketNoteEntity, TicketNote>
    {
        public TicketNotesController(IGenericRepository<TicketNoteEntity> repo) : base(repo) { }

        protected override TicketNote MapEntityToModel(TicketNoteEntity entity)
        {
            var model = DBMapper.MapTicketNoteEntity2TicketNote(entity);
            return model;

        }

        protected override void MapModelToEntity(TicketNote model, TicketNoteEntity entity) =>
            DBMapper.MapTicketNote2TicketNoteEntity(model, ref entity);

    }

}
