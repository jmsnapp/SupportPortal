using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationTypesController : GenericController<IntegrationTypeEntity, IntegrationType>
    {
        public IntegrationTypesController(IGenericRepository<IntegrationTypeEntity> repo, Mapper mapper) : base(repo, mapper) { }
    }
}
