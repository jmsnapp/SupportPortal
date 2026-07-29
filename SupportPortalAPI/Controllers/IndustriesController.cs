using SupportPortalInfrastructure;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IndustriesController : GenericController<IndustryEntity, Industry>
    {
        public IndustriesController(IGenericRepository<IndustryEntity> repo, DBMapper mapper) : base(repo, mapper) { }
    }
}
