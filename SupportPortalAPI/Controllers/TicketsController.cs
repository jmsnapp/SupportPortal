using System.Threading.Tasks;
using SupportPortalDomain;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalAPI.Controllers
{
    public class TicketsController : GenericController<TicketEntity, Ticket>
    {
        private readonly IGenericRepository<CustomerEntity> _customerRepo;
        private readonly IGenericRepository<IntegrationEntity> _integrationRepo;
        private readonly IGenericRepository<EscalationEntity> _escalationRepo;
        private readonly IGenericRepository<SeverityEntity> _severityRepo;
        private readonly IGenericRepository<IndustryEntity> _industry_repo;
        private readonly IGenericRepository<IntegrationTypeEntity> _integrationTypeRepo;
        private readonly IGenericRepository<IntegrationStatusEntity> _integrationStatus_repo;
        private readonly IGenericRepository<SupportStatusEntity> _supportStatus_repo;
        private readonly ITicketNoteRepository _ticketNoteRepo;

        public TicketsController(
            IGenericRepository<TicketEntity> repo,
            IGenericRepository<CustomerEntity> customerRepo,
            IGenericRepository<IntegrationEntity> integrationRepo,
            IGenericRepository<EscalationEntity> escalationRepo,
            IGenericRepository<SeverityEntity> severityRepo,
            IGenericRepository<IndustryEntity> industryRepo,
            IGenericRepository<IntegrationTypeEntity> integrationTypeRepo,
            IGenericRepository<IntegrationStatusEntity> integrationStatusRepo,
            IGenericRepository<SupportStatusEntity> supportStatusRepo,
            ITicketNoteRepository ticketNoteRepo,
            DBMapper mapper)
            : base(repo, mapper)
        {
            _customerRepo = customerRepo;
            _integrationRepo = integrationRepo;
            _escalationRepo = escalationRepo;
            _severityRepo = severityRepo;
            _industry_repo = industryRepo;
            _integrationTypeRepo = integrationTypeRepo;
            _integrationStatus_repo = integrationStatusRepo;
            _supportStatus_repo = supportStatusRepo;
            _ticketNoteRepo = ticketNoteRepo;
        }

        // The constructor code above intentionally wires the many repositories required
        // by Mapper.MapTicketEntity2Ticket. Implement the mapping below.

        protected override async Task<Ticket> MapEntityToModelAsync(TicketEntity entity)
        {
            var model = await _mapper.MapTicketEntity2TicketAsync(entity,
                                                      _customerRepo,
                                                      _integrationRepo,
                                                      _escalationRepo,
                                                      _severityRepo,
                                                      _industry_repo,
                                                      _integrationTypeRepo,
                                                      _integrationStatus_repo,
                                                      _supportStatus_repo,
                                                      _ticketNoteRepo);
            return model;
        }

        // NOTE: the ctor had duplicate assignments in the original; ensure injected parameter names match DI registrations.
    }
}
