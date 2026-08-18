using System.Threading;
using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationsController : GenericController<IntegrationEntity, Integration>
    {
        public IntegrationsController(IGenericRepository<IntegrationEntity> repo) : base(repo)
        {}

        protected override Integration MapEntityToModel(IntegrationEntity entity)
        {
            var model = DBMapper.MapIntegrationEntity2Integration(entity);
            return model;

        }

        protected override void MapModelToEntity(Integration model, IntegrationEntity entity) =>
            DBMapper.MapIntegration2IntegrationEntity(model, ref entity);

    }

}
