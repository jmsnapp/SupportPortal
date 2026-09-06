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
    public class LinkProjectPhasesController : GenericController<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>
    {
        public LinkProjectPhasesController(IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options)
        { }

        // GET api/[controller]/active?projectId=1&page=1&pageSize=50
        [HttpGet("active")]
        public async Task<ActionResult<PagedResult<ProjectPhase>>> GetAllActive([FromQuery] Int64 projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = DEFAULT_PAGE_SIZE, CancellationToken ct = default)
        {
            List<ProjectPhase> lstResult = await _repo.GetByParentIdAsync(projectId, ct);

            ActionResult<PagedResult<ProjectPhase>> lstPageResult = Collection(lstResult, page, pageSize, ct);

            return lstPageResult;

        }

        protected override void MapModelToEntity(ProjectPhase model, LinkProjectPhaseEntity entity) =>
            DBMapper.MapProjectPhase2LinkProjectPhaseEntity(model, ref entity);

    }

}
