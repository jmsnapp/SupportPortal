using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Configuration;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class EscalationsController : GenericController<Escalation, Escalation, EscalationEntity>
    {
        public EscalationsController(IGenericRepository<Escalation, Escalation, EscalationEntity> repo, IOptions<PaginationOptions>? options = null) : base(repo, options) { }

        protected override void MapModelToEntity(Escalation model, EscalationEntity entity) =>
            DBMapper.MapEscalation2EscalationEntity(model, ref entity);

    }

}
