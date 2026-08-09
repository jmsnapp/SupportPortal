using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalDomain
{
    public class DBMapper
    {
        // Synchronous wrappers remain for backward compatibility (they block),
        // but async variants below are used by controllers to avoid blocking on async repo calls.

        public Customer MapCustomerEntity2Customer(CustomerEntity entity, IGenericRepository<IndustryEntity> industryRepository)
            => MapCustomerEntity2CustomerAsync(entity, industryRepository).GetAwaiter().GetResult();

        public async Task<Customer> MapCustomerEntity2CustomerAsync(CustomerEntity entity, IGenericRepository<IndustryEntity> industryRepository)
        {
            var customer = new Customer();
            MapPortalEntity2Object(entity, customer);

            var industryEntity = await industryRepository.GetByIdAsync(entity.IndustryId).ConfigureAwait(false);
            MapPortalEntity2Object(industryEntity, customer.Industry);

            customer.PrimaryContact = entity.PrimaryContactName;
            customer.PrimaryContactEmail = entity.PrimaryContactEmail;
            customer.TechnicalContact = entity.TechnicalContactName;
            customer.TechnicalContactEmail = entity.TechnicalContactEmail;
            customer.CreatedDate = entity.CreatedDate;

            return customer;

        }

        public Integration MapIntegrationEntity2Integration(IntegrationEntity entity,
                                                            IGenericRepository<IntegrationTypeEntity> integrationTypeRepository,
                                                            IGenericRepository<IntegrationStatusEntity> integrationStatusRepository,
                                                            IGenericRepository<CustomerEntity> customerRepository,
                                                            IGenericRepository<IndustryEntity> industryRepository)
            => MapIntegrationEntity2IntegrationAsync(entity, integrationTypeRepository, integrationStatusRepository, customerRepository, industryRepository).GetAwaiter().GetResult();

        public async Task<Integration> MapIntegrationEntity2IntegrationAsync(IntegrationEntity entity,
                                                                             IGenericRepository<IntegrationTypeEntity> integrationTypeRepository,
                                                                             IGenericRepository<IntegrationStatusEntity> integrationStatusRepository,
                                                                             IGenericRepository<CustomerEntity> customerRepository,
                                                                             IGenericRepository<IndustryEntity> industryRepository)
        {
            var integration = new Integration();
            MapPortalEntity2Object(entity, integration);

            var typeEntity = await integrationTypeRepository.GetByIdAsync(entity.IntegrationTypeId).ConfigureAwait(false);
            var statusEntity = await integrationStatusRepository.GetByIdAsync(entity.CurrentStatusId).ConfigureAwait(false);
            var customerEntity = await customerRepository.GetByIdAsync(entity.CustomerId).ConfigureAwait(false);

            integration.Customer = await MapCustomerEntity2CustomerAsync(customerEntity, industryRepository).ConfigureAwait(false);
            MapPortalEntity2Object(typeEntity, integration.Type);
            MapPortalEntity2Object(statusEntity, integration.CurrentStatus);

            integration.LastSuccessfulSync = entity.LastSuccessfulSync;
            integration.LastFailedSync = entity.LastFailedSync;
            integration.RetryCount = entity.RetryCount;

            return integration;

        }

        public ProjectPhase MapLinkProjectPhaseEntity2ProjectPhase(LinkProjectPhaseEntity entity,
                                                                   IGenericRepository<PhaseEntity> phaseRepository)
            => MapLinkProjectPhaseEntity2ProjectPhaseAsync(entity, phaseRepository).GetAwaiter().GetResult();

        public async Task<ProjectPhase> MapLinkProjectPhaseEntity2ProjectPhaseAsync(LinkProjectPhaseEntity entity,
                                                                                     IGenericRepository<PhaseEntity> phaseRepository)
        {
            var projectPhase = new ProjectPhase();
            MapPortalEntity2Object(entity, projectPhase);

            var phaseEntity = await phaseRepository.GetByIdAsync(entity.PhaseId).ConfigureAwait(false);
            MapPortalEntity2Object(phaseEntity, projectPhase.Phase);

            projectPhase.ProjectId = entity.ProjectId;
            projectPhase.Percentage = entity.Percentage;
            projectPhase.Order = entity.Order;

            return projectPhase;

        }

        public Project MapProjectEntity2Project(ProjectEntity entity,
                                                ILinkProjectPhaseRepository projectPhaseRepository,
                                                IGenericRepository<PhaseEntity> phaseRepository,
                                                IProjectNoteRepository projectNoteRepository)
            => MapProjectEntity2ProjectAsync(entity, projectPhaseRepository, phaseRepository, projectNoteRepository).GetAwaiter().GetResult();

        public async Task<Project> MapProjectEntity2ProjectAsync(ProjectEntity entity,
                                                                 ILinkProjectPhaseRepository projectPhaseRepository,
                                                                 IGenericRepository<PhaseEntity> phaseRepository,
                                                                 IProjectNoteRepository projectNoteRepository)
        {
            var project = new Project();
            MapPortalEntity2Object(entity, project);

            var currentPhaseEntity = await phaseRepository.GetByIdAsync(entity.CurrentPhase).ConfigureAwait(false);
            var lstProjectPhaseEntities = await projectPhaseRepository.GetByProjectIdAsync(entity.Id).ConfigureAwait(false);
            var lstProjectNoteEntities = await projectNoteRepository.GetByProjectIdAsync(entity.Id).ConfigureAwait(false);

            MapPortalEntity2Object(currentPhaseEntity, project.CurrentPhase);

            foreach (var linkProjectPhaseEntity in lstProjectPhaseEntities)
            {
                var projectPhase = await MapLinkProjectPhaseEntity2ProjectPhaseAsync(linkProjectPhaseEntity, phaseRepository).ConfigureAwait(false);
                project.Phases.Add(projectPhase);

            }

            foreach (var projectNoteEntity in lstProjectNoteEntities)
            {
                var projectNote = MapProjectNoteEntity2ProjectNote(projectNoteEntity);
                project.Notes.Add(projectNote);

            }

            project.TargetGoLive = entity.TargetGoLiveDate;
            project.ActualGoLive = entity.ActualGoLiveDate;

            return project;

        }

        public Ticket MapTicketEntity2Ticket(TicketEntity entity,
                                             IGenericRepository<CustomerEntity> customerRepository,
                                             IGenericRepository<IntegrationEntity> integrationRepository,
                                             IGenericRepository<EscalationEntity> escalationRepository,
                                             IGenericRepository<SeverityEntity> severityRepository,
                                             IGenericRepository<IndustryEntity> industryRepository,
                                             IGenericRepository<IntegrationTypeEntity> integrationTypeRepository,
                                             IGenericRepository<IntegrationStatusEntity> integrationStatusRepository,
                                             IGenericRepository<SupportStatusEntity> supportStatusRepository,
                                             ITicketNoteRepository ticketNoteRepository)
            => MapTicketEntity2TicketAsync(entity,
                                           customerRepository,
                                           integrationRepository,
                                           escalationRepository,
                                           severityRepository,
                                           industryRepository,
                                           integrationTypeRepository,
                                           integrationStatusRepository,
                                           supportStatusRepository,
                                           ticketNoteRepository).GetAwaiter().GetResult();

        public async Task<Ticket> MapTicketEntity2TicketAsync(TicketEntity entity,
            IGenericRepository<CustomerEntity> customerRepository,
            IGenericRepository<IntegrationEntity> integrationRepository,
            IGenericRepository<EscalationEntity> escalationRepository,
            IGenericRepository<SeverityEntity> severityRepository,
            IGenericRepository<IndustryEntity> industryRepository,
            IGenericRepository<IntegrationTypeEntity> integrationTypeRepository,
            IGenericRepository<IntegrationStatusEntity> integrationStatusRepository,
            IGenericRepository<SupportStatusEntity> supportStatusRepository,
            ITicketNoteRepository ticketNoteRepository)
        {
            var ticket = new Ticket();
            MapPortalEntity2Object(entity, ticket);

            var customerEntity = await customerRepository.GetByIdAsync(entity.CustomerId).ConfigureAwait(false);
            var integrationEntity = await integrationRepository.GetByIdAsync(entity.IntegrationId).ConfigureAwait(false);
            var severityEntity = await severityRepository.GetByIdAsync(entity.SeverityId).ConfigureAwait(false);
            var statusEntity = await supportStatusRepository.GetByIdAsync(entity.StatusId).ConfigureAwait(false);
            var lstTicketNoteEntities = await ticketNoteRepository.GetByTicketIdAsync(entity.Id).ConfigureAwait(false);

            ticket.Customer = await MapCustomerEntity2CustomerAsync(customerEntity, industryRepository).ConfigureAwait(false);

            ticket.Integration = await MapIntegrationEntity2IntegrationAsync(integrationEntity,
                                                                             integrationTypeRepository,
                                                                             integrationStatusRepository,
                                                                             customerRepository,
                                                                             industryRepository).ConfigureAwait(false);

            MapPortalEntity2Object(severityEntity, ticket.Severity);
            MapPortalEntity2Object(statusEntity, ticket.Status);

            if (entity.EscalationId.HasValue)
            {
                var escalationEntity = await escalationRepository.GetByIdAsync(entity.EscalationId.Value).ConfigureAwait(false);
                ticket.Escalation = MapEscalationEntity2Escalation(escalationEntity);
            }

            foreach (var ticketNoteEntity in lstTicketNoteEntities)
            {
                var ticketNote = MapTicketNoteEntity2TicketNote(ticketNoteEntity);
                ticket.Notes.Add(ticketNote);
            }

            ticket.Reproduce = entity.Reproduce;
            ticket.ReportedBy = entity.ReportedBy;
            ticket.AssignedTo = entity.AssignedTo;
            ticket.CreatedDate = entity.CreatedDate;
            ticket.ResolutionDate = entity.ResolutionDate;
            ticket.Resolution = entity.Resolution;

            return ticket;

        }

        public static CustomerEntity MapCustomer2CustomerEntity(Customer customer)
        {
            CustomerEntity entity = new CustomerEntity();
            MapPortalObject2Entity(customer, entity);

            entity.IndustryId = customer.Industry.Id;
            entity.PrimaryContactName = customer.PrimaryContact;
            entity.PrimaryContactEmail = customer.PrimaryContactEmail;
            entity.TechnicalContactName = customer.TechnicalContact;
            entity.TechnicalContactEmail = customer.TechnicalContactEmail;
            entity.CreatedDate = customer.CreatedDate;

            return entity;

        }

        public static Escalation MapEscalationEntity2Escalation(EscalationEntity entity)
        {
            var objReturn = new Escalation();
            MapPortalEntity2Object(entity, objReturn);

            objReturn.ProblemSummary = entity.ProblemSummary;
            objReturn.CustomerImpact = entity.CustomerImpact;
            objReturn.RecommendedActions = entity.RecommendedActions;
            objReturn.RootCause = entity.RootCause;

            return objReturn;

        }

        public static EscalationEntity MapEscalation2EscalationEntity(Escalation obj)
        {
            var entityReturn = new EscalationEntity();
            MapPortalObject2Entity(obj, entityReturn);

            entityReturn.ProblemSummary = obj.ProblemSummary;
            entityReturn.CustomerImpact = obj.CustomerImpact;
            entityReturn.RecommendedActions = obj.RecommendedActions;
            entityReturn.RootCause = obj.RootCause;

            return entityReturn;

        }

        public static IntegrationEntity MapIntegration2IntegrationEntity(Integration obj)
        {
            var entity = new IntegrationEntity();
            MapPortalObject2Entity(obj, entity);

            entity.IntegrationTypeId = obj.Type.Id;
            entity.CurrentStatusId = obj.CurrentStatus.Id;
            entity.CustomerId = obj.Customer.Id;

            entity.LastSuccessfulSync = obj.LastSuccessfulSync;
            entity.LastFailedSync = obj.LastFailedSync;
            entity.RetryCount = obj.RetryCount;

            return entity;

        }

        public static LinkProjectPhaseEntity MapProjectPhase2LinkProjectPhaseEntity(ProjectPhase obj)
        {
            var entity = new LinkProjectPhaseEntity();
            MapPortalObject2Entity(obj, entity);

            entity.ProjectId = obj.ProjectId;
            entity.PhaseId = obj.Phase.Id;
            entity.Percentage = obj.Percentage;
            entity.Order = obj.Order;

            return entity;

        }

        public static ProjectEntity MapProject2ProjectEntity(Project obj)
        {
            var entity = new ProjectEntity();
            MapPortalObject2Entity(obj, entity);

            entity.CurrentPhase = obj.CurrentPhase.Id;
            entity.TargetGoLiveDate = obj.TargetGoLive;
            entity.ActualGoLiveDate = obj.ActualGoLive;

            return entity;

        }

        public static ProjectNote MapProjectNoteEntity2ProjectNote(ProjectNoteEntity entity)
        {
            var projectNote = new ProjectNote();
            MapPortalEntity2Object(entity, projectNote);

            projectNote.ProjectId = entity.ProjectId;
            projectNote.Note = entity.Note;

            return projectNote;

        }

        public static ProjectNoteEntity MapProjectNote2ProjectNoteEntity(ProjectNote obj)
        {
            var entity = new ProjectNoteEntity();
            MapPortalObject2Entity(obj, entity);

            entity.ProjectId = obj.ProjectId;
            entity.Note = obj.Note;

            return entity;

        }

        public static TicketEntity MapTicket2TicketEntity(Ticket obj)
        {
            var entity = new TicketEntity();
            MapPortalObject2Entity(obj, entity);

            entity.CustomerId = obj.Customer.Id;
            entity.IntegrationId = obj.Integration.Id;
            entity.SeverityId = obj.Severity.Id;
            entity.StatusId = obj.Status.Id;
            entity.Reproduce = obj.Reproduce;
            entity.ReportedBy = obj.ReportedBy;
            entity.AssignedTo = obj.AssignedTo;
            entity.CreatedDate = obj.CreatedDate;
            entity.ResolutionDate = obj.ResolutionDate;
            entity.Resolution = obj.Resolution;
            entity.EscalationId = obj.Escalation?.Id;

            return entity;

        }

        public static TicketNote MapTicketNoteEntity2TicketNote(TicketNoteEntity entity)
        {
            var ticketNote = new TicketNote();
            MapPortalEntity2Object(entity, ticketNote);

            ticketNote.TicketId = entity.TicketId;
            ticketNote.Note = entity.Note;

            return ticketNote;

        }

        public static TicketNoteEntity MapTicketNote2TicketNoteEntity(TicketNote obj)
        {
            var entity = new TicketNoteEntity();
            MapPortalObject2Entity(obj, entity);

            entity.TicketId = obj.TicketId;
            entity.Note = obj.Note;

            return entity;

        }

        // Portal entity/object mapping helpers
        public static void MapPortalEntity2Object(PortalEntity entity, PortalObject obj)
        {
            obj.Id = entity.Id;
            obj.Name = entity.Name;
            obj.Description = entity.Description;
            obj.Deleted = entity.Deleted;

        }

        private static void MapPortalObject2Entity(PortalObject obj, PortalEntity entity)
        {
            entity.Id = obj.Id;
            entity.Name = obj.Name;
            entity.Description = obj.Description;
            entity.Deleted = obj.Deleted;

        }

    }

}
