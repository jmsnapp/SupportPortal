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
        private readonly IGenericRepository<IndustryEntity> _industryRepo;
        private readonly IGenericRepository<IntegrationTypeEntity> _integrationTypeRepo;
        private readonly IGenericRepository<IntegrationStatusEntity> _integrationStatusRepo;
        private readonly IGenericRepository<SupportStatusEntity> _supportStatusRepo;
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
            _industryRepo = industryRepo;
            _integrationTypeRepo = integrationTypeRepo;
            _integrationTypeRepo = integrationTypeRepo; 
            _integrationStatusRepo = integrationStatusRepo;
            _supportStatusRepo = supportStatusRepo;
            _ticketNoteRepo = ticketNoteRepo;
        }

        // The constructor code above intentionally wires the many repositories required
        // by Mapper.MapTicketEntity2Ticket. Implement the mapping below.

        protected override Task<Ticket> MapEntityToModelAsync(TicketEntity entity)
        {
            var model = _mapper.MapTicketEntity2Ticket(entity,
                                                      _customerRepo,
                                                      _integrationRepo,
                                                      _escalationRepo,
                                                      _severityRepo,
                                                      _industryRepo,
                                                      _integrationTypeRepo,
                                                      _integrationStatusRepo,
                                                      _supportStatusRepo,
                                                      _ticketNoteRepo);
            return Task.FromResult(model);
        }

        // NOTE: there are duplicate assignments in the ctor for integration repo fields; ensure the injected parameter names match actual DI registrations.
    }
}
