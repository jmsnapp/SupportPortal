using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IndustriesController : GenericController<IndustryEntity, Industry>
    {
        public IndustriesController(IGenericRepository<IndustryEntity> repo, Mapper mapper) : base(repo, mapper) { }
    }
}
