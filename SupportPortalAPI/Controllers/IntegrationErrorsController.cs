using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationErrorsController : GenericController<IntegrationErrorEntity, IntegrationError>
    {
        public IntegrationErrorsController(IGenericRepository<IntegrationErrorEntity> repo, DBMapper mapper) : base(repo, mapper) { }
    }
}
