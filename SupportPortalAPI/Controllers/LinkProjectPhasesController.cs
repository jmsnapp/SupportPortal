using System.Threading.Tasks;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class LinkProjectPhasesController : GenericController<LinkProjectPhaseEntity, ProjectPhase>
    {
        private readonly IGenericRepository<PhaseEntity> _phaseRepo;

        public LinkProjectPhasesController(IGenericRepository<LinkProjectPhaseEntity> repo, IGenericRepository<PhaseEntity> phaseRepo, DBMapper mapper)
            : base(repo, mapper)
        {
            _phaseRepo = phaseRepo;
        }

        protected override Task<ProjectPhase> MapEntityToModelAsync(LinkProjectPhaseEntity entity)
        {
            var model = _mapper.MapLinkProjectPhaseEntity2ProjectPhase(entity, _phaseRepo);
            return Task.FromResult(model);
        }
    }
}
