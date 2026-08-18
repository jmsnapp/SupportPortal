using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class PhasesController : GenericController<PhaseEntity, Phase>
    {
        public PhasesController(IGenericRepository<PhaseEntity> repo) : base(repo) { }

    }

}
