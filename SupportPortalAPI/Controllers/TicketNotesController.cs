using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Configuration;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class TicketNotesController : GenericController<TicketNote, TicketNote, TicketNoteEntity>
    {
        public TicketNotesController(IGenericRepository<TicketNote, TicketNote, TicketNoteEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options) { }

        // GET api/[controller]/active?ticketId=1&page=1&pageSize=50
        [HttpGet("active")]
        public async Task<ActionResult<PagedResult<TicketNote>>> GetAllActive([FromQuery] Int64 ticketId, [FromQuery] int page = 1, [FromQuery] int pageSize = DEFAULT_PAGE_SIZE, CancellationToken ct = default)
        {
            List<TicketNote> lstResult = await _repo.GetByParentIdAsync(ticketId, ct);

            ActionResult<PagedResult<TicketNote>> lstPageResult = Collection(lstResult, page, pageSize, ct);

            return lstPageResult;

        }

        protected override void MapModelToEntity(TicketNote model, TicketNoteEntity entity) =>
            DBMapper.MapTicketNote2TicketNoteEntity(model, ref entity);

    }

}
