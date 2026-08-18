using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class ProjectsController : GenericController<ProjectEntity, Project>
    {
        public ProjectsController (IGenericRepository<ProjectEntity> repo) : base(repo)
        { }

        protected override Project MapEntityToModel(ProjectEntity entity)
        {
            var model = DBMapper.MapProjectEntity2Project(entity);
            return model;

        }

        protected override void MapModelToEntity(Project model, ProjectEntity entity) =>
            DBMapper.MapProject2ProjectEntity(model, ref entity);

    }

}
