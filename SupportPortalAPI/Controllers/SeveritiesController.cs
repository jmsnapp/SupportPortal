using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class SeveritiesController : GenericController<SeverityEntity, Severity>
    {
        public SeveritiesController(IGenericRepository<SeverityEntity> repo, Mapper mapper) : base(repo, mapper) { }
    }
}
