using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationErrorsController : GenericController<IntegrationErrorEntity, SupportPortalInfrastructure.Models.IntegrationError>
    {
        public IntegrationErrorsController(IGenericRepository<IntegrationErrorEntity> repo, Mapper mapper) : base(repo, mapper) { }
    }
}
