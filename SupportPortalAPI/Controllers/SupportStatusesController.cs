using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Configuration;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class SupportStatusesController : GenericController<SupportStatus, SupportStatus, SupportStatusEntity>
    {
        public SupportStatusesController(IGenericRepository<SupportStatus, SupportStatus, SupportStatusEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options) { }

        // GET api/[controller]/by-name/{name}
        [HttpGet("by-name/{name}")]
        public virtual async Task<IActionResult> GetByName(string name, CancellationToken ct = default)
        {
            var model = await _repo.GetByNameAsync(name, ct);
            if (model == null) return NotFound();

            return Ok(model);

        }

    }

}
