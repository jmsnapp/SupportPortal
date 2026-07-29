using System.Threading.Tasks;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class TicketNotesController : GenericController<TicketNoteEntity, TicketNote>
    {
        public TicketNotesController(IGenericRepository<TicketNoteEntity> repo, DBMapper mapper) : base(repo, mapper) { }

        protected override Task<TicketNote> MapEntityToModelAsync(TicketNoteEntity entity)
        {
            var model = DBMapper.MapTicketNoteEntity2TicketNote(entity);
            return Task.FromResult(model);
        }
    }
}
