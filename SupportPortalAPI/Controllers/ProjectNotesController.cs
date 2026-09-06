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
    public class ProjectNotesController : GenericController<ProjectNote, ProjectNote, ProjectNoteEntity>
    {
        public ProjectNotesController(IGenericRepository<ProjectNote, ProjectNote, ProjectNoteEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options) { }

        // GET api/[controller]/active?projectId=1&page=1&pageSize=50
        [HttpGet("active")]
        public async Task<ActionResult<PagedResult<ProjectNote>>> GetAllActive([FromQuery] Int64 projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = DEFAULT_PAGE_SIZE, CancellationToken ct = default)
        {
            List<ProjectNote> lstResult = await _repo.GetByParentIdAsync(projectId, ct);

            ActionResult<PagedResult<ProjectNote>> lstPageResult = Collection(lstResult, page, pageSize, ct);

            return lstPageResult;

        }

        protected override void MapModelToEntity(ProjectNote model, ProjectNoteEntity entity) =>
            DBMapper.MapProjectNote2ProjectNoteEntity(model, ref entity);

    }

}
