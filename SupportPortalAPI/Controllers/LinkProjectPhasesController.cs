using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class LinkProjectPhasesController : GenericController<LinkProjectPhaseEntity, ProjectPhase>
    {
        public LinkProjectPhasesController(IGenericRepository<LinkProjectPhaseEntity> repo) : base(repo)
        {}

        protected override ProjectPhase MapEntityToModel(LinkProjectPhaseEntity entity)
        {
            var model = DBMapper.MapLinkProjectPhaseEntity2ProjectPhase(entity);
            return model;

        }

        protected override void MapModelToEntity(ProjectPhase model, LinkProjectPhaseEntity entity) =>
            DBMapper.MapProjectPhase2LinkProjectPhaseEntity(model, ref entity);

    }

}
