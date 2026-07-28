using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class PhasesController : GenericController<PhaseEntity, Phase>
    {
        public PhasesController(IGenericRepository<PhaseEntity> repo, Mapper mapper) : base(repo, mapper) { }
    }
}
