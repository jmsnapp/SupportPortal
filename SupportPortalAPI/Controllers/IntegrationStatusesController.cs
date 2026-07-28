using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationStatusesController : GenericController<IntegrationStatusEntity, IntegrationStatus>
    {
        public IntegrationStatusesController(IGenericRepository<IntegrationStatusEntity> repo, Mapper mapper) : base(repo, mapper) { }
    }
}
