using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class SupportStatusesController : GenericController<SupportStatusEntity, SupportStatus>
    {
        public SupportStatusesController(IGenericRepository<SupportStatusEntity> repo, Mapper mapper) : base(repo, mapper) { }
    }
}
