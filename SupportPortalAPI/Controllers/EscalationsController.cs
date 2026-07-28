using System.Threading.Tasks;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class EscalationsController : GenericController<EscalationEntity, Escalation>
    {
        public EscalationsController(IGenericRepository<EscalationEntity> repo, Mapper mapper) : base(repo, mapper) { }

        protected override Task<Escalation> MapEntityToModelAsync(EscalationEntity entity)
        {
            var model = Mapper.MapEscalationEntity2Escalation(entity);
            return Task.FromResult(model);
        }
    }
}
