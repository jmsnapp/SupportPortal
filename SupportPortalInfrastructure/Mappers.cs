using System;
using System.Collections.Generic;
using System.Text;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Models;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalInfrastructure
{
    public class Mapper
    {
        public Customer MapCustomerEntity2Customer(CustomerEntity entity, IGenericRepository<IndustryEntity> industryRepository)
        {
            // Implementation for mapping customer entity to a Customer
            Customer customer = new Customer();
            MapPortalEntity2Object(entity, customer);

            IndustryEntity industryEntity = industryRepository.GetByIdAsync(entity.IndustryId).Result;
            MapPortalEntity2Object(industryEntity, customer.Industry);

            customer.PrimaryContact = entity.PrimaryContact;
            customer.PrimaryContactEmail = entity.PrimaryContactEmail;
            customer.TechnicalContact = entity.TechnicalContact;
            customer.TechnicalContactEmail = entity.TechnicalContactEmail;
            customer.CreatedDate = entity.CreatedDate;

            return customer;

        }

        public Integration MapIntegrationEntity2Integration(IntegrationEntity entity, 
                                                            IGenericRepository<IntegrationTypeEntity> integrationTypeRepository, 
                                                            IGenericRepository<IntegrationStatusEntity> integrationStatusRepository, 
                                                            IGenericRepository<CustomerEntity> customerRepository,
                                                            IGenericRepository<IndustryEntity> industryRepository)
        {
            // Implementation for mapping integration entity to an Integration
            Integration integration = new Integration();
            MapPortalEntity2Object(entity, integration);

            IntegrationTypeEntity typeEntity = integrationTypeRepository.GetByIdAsync(entity.IntegrationTypeId).Result;
            IntegrationStatusEntity statusEntity = integrationStatusRepository.GetByIdAsync(entity.CurrentStatusId).Result;
            CustomerEntity customerEntity = customerRepository.GetByIdAsync(entity.CustomerId).Result;

            integration.Customer = MapCustomerEntity2Customer(customerEntity, industryRepository);
            MapPortalEntity2Object(typeEntity, integration.Type);
            MapPortalEntity2Object(statusEntity, integration.CurrentStatus);

            integration.LastSuccessfulSync = entity.LastSuccessfulSync;
            integration.LastFailedSync = entity.LastFailedSync;
            integration.RetryCount = entity.RetryCount;

            return integration;

        }

        public ProjectPhase MapLinkProjectPhaseEntity2ProjectPhase(LinkProjectPhaseEntity entity, 
                                                                   IGenericRepository<PhaseEntity> phaseRepository)
        {
            ProjectPhase projectPhase = new ProjectPhase();
            MapPortalEntity2Object(entity, projectPhase);

            PhaseEntity phaseEntity = phaseRepository.GetByIdAsync(entity.PhaseId).Result;
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
        {
            Project project = new Project();
            MapPortalEntity2Object(entity, project);


            PhaseEntity currentPhaseEntity = phaseRepository.GetByIdAsync(entity.CurrentPhase).Result;
            MapPortalEntity2Object(currentPhaseEntity, project.CurrentPhase);

            List<LinkProjectPhaseEntity> lstProjectPhaseEntities = projectPhaseRepository.GetByProjectIdAsync(entity.Id).Result.ToList();
            foreach (var linkProjectPhaseEntity in lstProjectPhaseEntities)
            {
                ProjectPhase projectPhase = MapLinkProjectPhaseEntity2ProjectPhase(linkProjectPhaseEntity, phaseRepository);
                project.Phases.Add(projectPhase);
            }

            List<ProjectNoteEntity> lstProjectNoteEntities = projectNoteRepository.GetByProjectIdAsync(entity.Id).Result.ToList();
            foreach (var projectNoteEntity in lstProjectNoteEntities)
            {
                ProjectNote projectNote = MapProjectNoteEntity2ProjectNote(projectNoteEntity);
                project.Notes.Add(projectNote);

            }

            project.TargetGoLive = entity.TargetGoLive;
            project.ActualGoLive = entity.ActualGoLive;

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
        {
            Ticket ticket = new Ticket();
            MapPortalEntity2Object(entity, ticket);

            CustomerEntity customerEntity = customerRepository.GetByIdAsync(entity.CustomerId).Result;
            ticket.Customer = MapCustomerEntity2Customer(customerEntity, industryRepository);

            IntegrationEntity integrationEntity = integrationRepository.GetByIdAsync(entity.IntegrationId).Result;
            ticket.Integration = MapIntegrationEntity2Integration(integrationEntity,
                                                                  integrationTypeRepository,
                                                                  integrationStatusRepository,
                                                                  customerRepository,
                                                                  industryRepository);

            SeverityEntity severityEntity = severityRepository.GetByIdAsync(entity.SeverityId).Result;
            MapPortalEntity2Object(severityEntity, ticket.Severity);

            SupportStatusEntity statusEntity = supportStatusRepository.GetByIdAsync(entity.SupportStatusId).Result;
            MapPortalEntity2Object(statusEntity, ticket.Status);

            if (entity.EscalationId.HasValue)
            {
                EscalationEntity escalationEntity = escalationRepository.GetByIdAsync(entity.EscalationId.Value).Result;
                ticket.Escalation = MapEscalationEntity2Escalation(escalationEntity);

            }

            List<TicketNoteEntity> lstTicketNoteEntities = ticketNoteRepository.GetByTicketIdAsync(entity.Id).Result.ToList();
            foreach (var ticketNoteEntity in lstTicketNoteEntities)
            {
                TicketNote ticketNote = MapTicketNoteEntity2TicketNote(ticketNoteEntity);
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
            // Implementation for mapping Customer to a customer entity
            CustomerEntity entity = new CustomerEntity();
            MapPortalObject2Entity(customer, entity);

            entity.IndustryId = customer.Industry.Id;
            entity.PrimaryContact = customer.PrimaryContact;
            entity.PrimaryContactEmail = customer.PrimaryContactEmail;
            entity.TechnicalContact = customer.TechnicalContact;
            entity.TechnicalContactEmail = customer.TechnicalContactEmail;
            entity.CreatedDate = customer.CreatedDate;

            return entity;

        }

        public static Escalation MapEscalationEntity2Escalation(EscalationEntity entity) 
        { 
            Escalation objReturn = new Escalation();
            MapPortalEntity2Object(entity, objReturn);

            objReturn.ProblemSummary = entity.ProblemSummary;
            objReturn.CustomerImpact = entity.CustomerImpact;
            objReturn.RecommendedAction = entity.RecommendedAction;
            objReturn.RootCause = entity.RootCause;

            return objReturn;

        }

        public static EscalationEntity MapEscalation2EscalationEntity(Escalation obj)
        {
            EscalationEntity entityReturn = new EscalationEntity();
            MapPortalObject2Entity(obj, entityReturn);

            entityReturn.ProblemSummary = obj.ProblemSummary;
            entityReturn.CustomerImpact = obj.CustomerImpact;
            entityReturn.RecommendedAction = obj.RecommendedAction;
            entityReturn.RootCause = obj.RootCause;

            return entityReturn;

        }

        public static IntegrationEntity MapIntegration2IntegrationEntity(Integration obj)
        {
            // Implementation for mapping Integration to an integration entity
            IntegrationEntity entity = new IntegrationEntity();
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
            // Implementation for mapping ProjectPhase to a link project phase entity
            LinkProjectPhaseEntity entity = new LinkProjectPhaseEntity();
            MapPortalObject2Entity(obj, entity);

            entity.ProjectId = obj.ProjectId;
            entity.PhaseId = obj.Phase.Id;
            entity.Percentage = obj.Percentage;
            entity.Order = obj.Order;

            return entity;

        }

        public static ProjectEntity MapProject2ProjectEntity(Project obj)
        {
            // Implementation for mapping Project to a project entity
            ProjectEntity entity = new ProjectEntity();
            MapPortalObject2Entity(obj, entity);

            entity.CurrentPhase = obj.CurrentPhase.Id;
            entity.TargetGoLive = obj.TargetGoLive;
            entity.ActualGoLive = obj.ActualGoLive;

            return entity;

        }

        public static ProjectNote MapProjectNoteEntity2ProjectNote(ProjectNoteEntity entity)
        {
            ProjectNote projectNote = new ProjectNote();
            MapPortalEntity2Object(entity, projectNote);

            projectNote.ProjectId = entity.ProjectId;
            projectNote.Note = entity.Note;

            return projectNote;

        }

        public static ProjectNoteEntity MapProjectNote2ProjectNoteEntity(ProjectNote obj)
        {
            ProjectNoteEntity entity = new ProjectNoteEntity();
            MapPortalObject2Entity(obj, entity);

            entity.ProjectId = obj.ProjectId;
            entity.Note = obj.Note;

            return entity;

        }

        public static TicketEntity MapTicket2TicketEntity(Ticket obj)
        {
            // Implementation for mapping Ticket to a ticket entity
            TicketEntity entity = new TicketEntity();
            MapPortalObject2Entity(obj, entity);

            entity.CustomerId = obj.Customer.Id;
            entity.IntegrationId = obj.Integration.Id;
            entity.SeverityId = obj.Severity.Id;          
            entity.SupportStatusId = obj.Status.Id;
            entity.Reproduce = obj.Reproduce;
            entity.ReportedBy = obj.ReportedBy;
            entity.AssignedTo = obj.AssignedTo;
            entity.CreatedDate = obj.CreatedDate;
            entity.ResolutionDate = obj.ResolutionDate;
            entity.Resolution = obj.Resolution;

            if (obj.Escalation != null)
                entity.EscalationId = obj.Escalation?.Id;

            else
                entity.EscalationId = null;

            return entity;

        }

        public static TicketNote MapTicketNoteEntity2TicketNote(TicketNoteEntity entity)
        {
            TicketNote ticketNote = new TicketNote();
            MapPortalEntity2Object(entity, ticketNote);

            ticketNote.TicketId = entity.TicketId;
            ticketNote.Note = entity.Note;

            return ticketNote;

        }

        public static TicketNoteEntity MapTicketNote2TicketNoteEntity(TicketNote obj)
        {
            TicketNoteEntity entity = new TicketNoteEntity();
            MapPortalObject2Entity(obj, entity);

            entity.TicketId = obj.TicketId;
            entity.Note = obj.Note;

            return entity;

        }

        public static void MapPortalEntity2Object(PortalEntity entity, PortalObject obj)
        {
            // Implementation for mapping portal entity to a PortalObject
            obj.Id = entity.Id;
            obj.Name = entity.Name;
            obj.Description = entity.Description;
            obj.Deleted = entity.Deleted;

        }

        public static void MapPortalObject2Entity(PortalObject obj, PortalEntity entity)
        {
            // Implementation for mapping PortalObject to a portal entity
            entity.Id = obj.Id;
            entity.Name = obj.Name;
            entity.Description = obj.Description;
            entity.Deleted = obj.Deleted;

        }

    }

}
