using System.Threading.Tasks;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class ProjectNotesController : GenericController<ProjectNoteEntity, ProjectNote>
    {
        public ProjectNotesController(IGenericRepository<ProjectNoteEntity> repo, DBMapper mapper)
            : base(repo, mapper) { }

        protected override Task<ProjectNote> MapEntityToModelAsync(ProjectNoteEntity entity)
        {
            var model = DBMapper.MapProjectNoteEntity2ProjectNote(entity);
            return Task.FromResult(model);
        }
    }
}
