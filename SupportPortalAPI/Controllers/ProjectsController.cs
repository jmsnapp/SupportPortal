using System.Threading.Tasks;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class ProjectsController : GenericController<ProjectEntity, Project>
    {
        private readonly ILinkProjectPhaseRepository _projectPhaseRepo;
        private readonly IGenericRepository<PhaseEntity> _phaseRepo;
        private readonly IProjectNoteRepository _projectNoteRepo;

        public ProjectsController(
            IGenericRepository<ProjectEntity> repo,
            ILinkProjectPhaseRepository projectPhaseRepo,
            IGenericRepository<PhaseEntity> phaseRepo,
            IProjectNoteRepository projectNoteRepo,
            DBMapper mapper)
            : base(repo, mapper)
        {
            _projectPhaseRepo = projectPhaseRepo;
            _phaseRepo = phaseRepo;
            _projectNoteRepo = projectNoteRepo;
        }

        protected override async Task<Project> MapEntityToModelAsync(ProjectEntity entity)
        {
            var model = await _mapper.MapProjectEntity2ProjectAsync(entity, _projectPhaseRepo, _phaseRepo, _projectNoteRepo);
            return model;
        }
    }
}
