using System.Threading;
using System.Threading.Tasks;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationsController : GenericController<IntegrationEntity, Integration>
    {
        private readonly IGenericRepository<IntegrationTypeEntity> _integrationTypeRepo;
        private readonly IGenericRepository<IntegrationStatusEntity> _integrationStatusRepo;
        private readonly IGenericRepository<CustomerEntity> _customerRepo;
        private readonly IGenericRepository<IndustryEntity> _industryRepo;

        public IntegrationsController(
            IGenericRepository<IntegrationEntity> repo,
            IGenericRepository<IntegrationTypeEntity> integrationTypeRepo,
            IGenericRepository<IntegrationStatusEntity> integrationStatusRepo,
            IGenericRepository<CustomerEntity> customerRepo,
            IGenericRepository<IndustryEntity> industryRepo,
            DBMapper mapper)
            : base(repo, mapper)
        {
            _integrationTypeRepo = integrationTypeRepo;
            _integrationStatusRepo = integrationStatusRepo;
            _customerRepo = customerRepo;
            _industryRepo = industryRepo;
        }

        protected override Task<Integration> MapEntityToModelAsync(IntegrationEntity entity)
        {
            var model = _mapper.MapIntegrationEntity2Integration(entity, _integrationTypeRepo, _integrationStatusRepo, _customerRepo, _industryRepo);
            return Task.FromResult(model);
        }
    }
}
