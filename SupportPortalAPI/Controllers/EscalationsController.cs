using Microsoft.AspNetCore.Mvc;
using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System.Threading.Tasks;

namespace SupportPortalAPI.Controllers
{
    public class EscalationsController : GenericController<EscalationEntity, Escalation>
    {
        public EscalationsController(IGenericRepository<EscalationEntity> repo) : base(repo) { }

        protected override Escalation MapEntityToModel(EscalationEntity entity)
        {
            var model = DBMapper.MapEscalationEntity2Escalation(entity);
            return model;

        }

        protected override void MapModelToEntity(Escalation model, EscalationEntity entity) =>
            DBMapper.MapEscalation2EscalationEntity(model, ref entity);

    }

}
