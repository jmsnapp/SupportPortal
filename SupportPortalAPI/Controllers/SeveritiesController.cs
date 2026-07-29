using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class SeveritiesController : GenericController<SeverityEntity, Severity>
    {
        public SeveritiesController(IGenericRepository<SeverityEntity> repo, DBMapper mapper) : base(repo, mapper) { }
    }
}
