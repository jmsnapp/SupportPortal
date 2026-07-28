using System.Threading;
using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class CustomersController : GenericController<CustomerEntity, Customer>
    {
        private readonly IGenericRepository<IndustryEntity> _industryRepo;

        public CustomersController(IGenericRepository<CustomerEntity> repo, IGenericRepository<IndustryEntity> industryRepo, Mapper mapper)
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
