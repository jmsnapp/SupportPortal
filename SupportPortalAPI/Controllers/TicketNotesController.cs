using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class TicketNotesController : GenericController<TicketNoteEntity, TicketNote>
    {
        public TicketNotesController(IGenericRepository<TicketNoteEntity> repo, Mapper mapper) : base(repo, mapper) { }

        protected override Task<TicketNote> MapEntityToModelAsync(TicketNoteEntity entity)
        {
            var model = Mapper.MapTicketNoteEntity2TicketNote(entity);
            return Task.FromResult(model);
        }
    }
}
