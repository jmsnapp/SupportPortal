using Microsoft.Extensions.Options;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Configuration;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class IntegrationErrorsController : GenericController<IntegrationError, IntegrationError, IntegrationErrorEntity>
    {
        public IntegrationErrorsController(IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options)
        { }

        protected override void MapModelToEntity(IntegrationError model, IntegrationErrorEntity entity) =>
            DBMapper.MapIntegrationError2IntegrationErrorEntity(model, ref entity);

    }

}
