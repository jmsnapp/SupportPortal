using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationStatusesController : GenericController<IntegrationStatusEntity, IntegrationStatus>
    {
        public IntegrationStatusesController(IGenericRepository<IntegrationStatusEntity> repo) : base(repo) { }

    }

}
