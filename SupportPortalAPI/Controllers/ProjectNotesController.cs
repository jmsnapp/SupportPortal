using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class ProjectNotesController : GenericController<ProjectNoteEntity, ProjectNote>
    {
        public ProjectNotesController(IGenericRepository<ProjectNoteEntity> repo) : base(repo) { }

        protected override ProjectNote MapEntityToModel(ProjectNoteEntity entity)
        {
            var model = DBMapper.MapProjectNoteEntity2ProjectNote(entity);
            return model;

        }

        protected override void MapModelToEntity(ProjectNote model, ProjectNoteEntity entity) =>
            DBMapper.MapProjectNote2ProjectNoteEntity(model, ref entity);

    }

}
