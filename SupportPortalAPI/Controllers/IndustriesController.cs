using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Configuration;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class IndustriesController : GenericController<Industry, Industry, IndustryEntity>
    {
        public IndustriesController(IGenericRepository<Industry, Industry, IndustryEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options) { }

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
