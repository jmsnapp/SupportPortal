using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationErrorsController : GenericController<IntegrationErrorEntity, IntegrationError>
    {
        public IntegrationErrorsController(IGenericRepository<IntegrationErrorEntity> repo) : base(repo)
        { }

        protected override IntegrationError MapEntityToModel(IntegrationErrorEntity entity)
        {
            // Use existing Mapper method that wires in industry repository
            var model = DBMapper.MapIntegrationErrorEntity2IntegrationError(entity);
            return model;

        }

        protected override void MapModelToEntity(IntegrationError model, IntegrationErrorEntity entity) =>
            DBMapper.MapIntegrationError2IntegrationErrorEntity(model, ref entity);

    }

}
