using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class ProjectNotesController : GenericController<ProjectNoteEntity, ProjectNote>
    {
        public ProjectNotesController(IGenericRepository<ProjectNoteEntity> repo, Mapper mapper)
            : base(repo, mapper) { }

        protected override Task<ProjectNote> MapEntityToModelAsync(ProjectNoteEntity entity)
        {
            var model = Mapper.MapProjectNoteEntity2ProjectNote(entity);
            return Task.FromResult(model);
        }
    }
}
