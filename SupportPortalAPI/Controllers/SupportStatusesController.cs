using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class SupportStatusesController : GenericController<SupportStatusEntity, SupportStatus>
    {
        public SupportStatusesController(IGenericRepository<SupportStatusEntity> repo, DBMapper mapper) : base(repo, mapper) { }
    }
}
