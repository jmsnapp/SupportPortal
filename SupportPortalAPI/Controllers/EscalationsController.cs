using Microsoft.AspNetCore.Mvc;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class EscalationsController : GenericController<EscalationEntity, Escalation>
    {
        public EscalationsController(IGenericRepository<EscalationEntity> repo, DBMapper mapper) : base(repo, mapper) { }

        protected override Task<Escalation> MapEntityToModelAsync(EscalationEntity entity)
        {
            var model = DBMapper.MapEscalationEntity2Escalation(entity);
            return Task.FromResult(model);
        }
    }
}
