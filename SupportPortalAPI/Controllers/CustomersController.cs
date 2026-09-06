using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Configuration;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class CustomersController : GenericController<Customer, CustomerListItem, CustomerEntity>
    {
        public CustomersController(IGenericRepository<Customer, CustomerListItem, CustomerEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options)
        { }

        // GET api/[controller]/by-name/{name}
        [HttpGet("by-name/{name}")]
        public virtual async Task<IActionResult> GetByName(string name, CancellationToken ct = default)
        {
            var model = await _repo.GetByNameAsync(name, ct);
            if (model == null) return NotFound();

            return Ok(model);

        }

        protected override void MapModelToEntity(Customer model, CustomerEntity entity) =>
            DBMapper.MapCustomer2CustomerEntity(model, ref entity);

    }

}
