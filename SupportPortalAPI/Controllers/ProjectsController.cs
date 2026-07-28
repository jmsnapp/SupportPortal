using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
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
            Mapper mapper)
            : base(repo, mapper)
        {
            _projectPhaseRepo = projectPhaseRepo;
            _phaseRepo = phaseRepo;
            _projectNoteRepo = projectNoteRepo;
        }

        protected override Task<Project> MapEntityToModelAsync(ProjectEntity entity)
        {
            var model = _mapper.MapProjectEntity2Project(entity, _projectPhaseRepo, _phaseRepo, _projectNoteRepo);
            return Task.FromResult(model);
        }
    }
}
