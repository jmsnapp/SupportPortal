using Microsoft.AspNetCore.Mvc;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class CustomersController : GenericController<CustomerEntity, Customer>
    {
        private readonly IGenericRepository<IndustryEntity> _industryRepo;

        public CustomersController(IGenericRepository<CustomerEntity> repo, IGenericRepository<IndustryEntity> industryRepo, DBMapper mapper)
            : base(repo, mapper)
        {
            _industryRepo = industryRepo;
        }

        protected override Task<Customer> MapEntityToModelAsync(CustomerEntity entity)
        {
            // Use existing Mapper method that wires in industry repository
            var model = _mapper.MapCustomerEntity2Customer(entity, _industryRepo);
            return Task.FromResult(model);
        }
    }
}
