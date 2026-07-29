using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationTypesController : GenericController<IntegrationTypeEntity, IntegrationType>
    {
        public IntegrationTypesController(IGenericRepository<IntegrationTypeEntity> repo, DBMapper mapper) : base(repo, mapper) { }
    }
}
