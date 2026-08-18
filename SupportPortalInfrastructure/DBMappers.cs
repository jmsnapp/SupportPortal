using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;

namespace SupportPortalInfrastructure
{
    public class DBMapper
    {
        public static Customer MapCustomerEntity2Customer(CustomerEntity? entity)
        {
            Customer customer = new Customer();
            if (entity == null) return customer;

            MapPortalEntity2Object(entity, customer);
            MapRef(entity.Industry, customer.Industry, entity.IndustryId);

            customer.PrimaryContact = entity.PrimaryContactName;
            customer.PrimaryContactEmail = entity.PrimaryContactEmail;
            customer.TechnicalContact = entity.TechnicalContactName;
            customer.TechnicalContactEmail = entity.TechnicalContactEmail;
            customer.CreatedDate = entity.CreatedDate;

            return customer;

        }

        public static void MapCustomer2CustomerEntity(Customer customer, ref CustomerEntity entity)
        {
            // Implementation for mapping Customer to a customer entity
            if (entity == null) 
                entity = new CustomerEntity();

            MapPortalObject2Entity(customer, entity);

            entity.IndustryId = Ref(customer.Industry);
            entity.PrimaryContactName = customer.PrimaryContact;
            entity.PrimaryContactEmail = customer.PrimaryContactEmail;
            entity.TechnicalContactName = customer.TechnicalContact;
            entity.TechnicalContactEmail = customer.TechnicalContactEmail;
            entity.CreatedDate = customer.CreatedDate;

        }

        public static Escalation MapEscalationEntity2Escalation(EscalationEntity? entity) 
        { 
            Escalation objReturn = new Escalation();
            if (entity == null) return objReturn;

            MapPortalEntity2Object(entity, objReturn);

            objReturn.ProblemSummary = entity.ProblemSummary;
            objReturn.CustomerImpact = entity.CustomerImpact;
            objReturn.RecommendedActions = entity.RecommendedActions;
            objReturn.RootCause = entity.RootCause;
            objReturn.CreatedDate = entity.CreatedDate;

            return objReturn;

        }

        public static void MapEscalation2EscalationEntity(Escalation obj, ref EscalationEntity entity)
        {
            if  (entity == null)
                entity = new EscalationEntity();

            MapPortalObject2Entity(obj, entity);

            entity.ProblemSummary = obj.ProblemSummary;
            entity.CustomerImpact = obj.CustomerImpact;
            entity.RecommendedActions = obj.RecommendedActions;
            entity.RootCause = obj.RootCause;
            entity.CreatedDate = obj.CreatedDate;

        }

        public static Integration MapIntegrationEntity2Integration(IntegrationEntity? entity)
        {
            // Implementation for mapping integration entity to an Integration
            Integration integration = new Integration();
            if (entity == null) return integration;

            MapPortalEntity2Object(entity, integration);
            MapRef(entity.IntegrationType, integration.Type, entity.IntegrationTypeId);
            MapRef(entity.CurrentStatus, integration.CurrentStatus, entity.CurrentStatusId);

            integration.Customer = MapCustomerEntity2Customer(entity.Customer);
            integration.Customer.Id = entity.CustomerId;

            integration.LastSuccessfulSync = entity.LastSuccessfulSync;
            integration.LastFailedSync = entity.LastFailedSync;
            integration.RetryCount = entity.RetryCount;

            return integration;

        }

        public static void MapIntegration2IntegrationEntity(Integration obj, ref IntegrationEntity entity)
        {
            if (entity == null)
                entity = new IntegrationEntity();

            MapPortalObject2Entity(obj, entity);

            entity.IntegrationTypeId = Ref(obj.Type);
            entity.CurrentStatusId = Ref(obj.CurrentStatus);
            entity.CustomerId = Ref(obj.Customer);

            entity.LastSuccessfulSync = obj.LastSuccessfulSync;
            entity.LastFailedSync = obj.LastFailedSync;
            entity.RetryCount = obj.RetryCount;

        }

        public static IntegrationError MapIntegrationErrorEntity2IntegrationError(IntegrationErrorEntity? entity)
        {
            IntegrationError objReturn = new IntegrationError();
            if (entity == null) return objReturn;

            MapPortalEntity2Object(entity, objReturn);
            MapRef(entity.Integration, objReturn.Integration, entity.IntegrationId);

            objReturn.ErrorMessage = entity.ErrorMessage;
            objReturn.ErrorTime = entity.ErrorTime;
            objReturn.StackTrace = entity.StackTrace;

            return objReturn;

        }

        public static void MapIntegrationError2IntegrationErrorEntity(IntegrationError obj, ref IntegrationErrorEntity entity)
        {
            if (entity == null)
                entity = new IntegrationErrorEntity();

            MapPortalObject2Entity(obj, entity);
            entity.IntegrationId = Ref(obj.Integration);
            entity.ErrorMessage = obj.ErrorMessage;
            entity.ErrorTime = obj.ErrorTime;
            entity.StackTrace = obj.StackTrace;

        }

        public static ProjectPhase MapLinkProjectPhaseEntity2ProjectPhase(LinkProjectPhaseEntity? entity)
        {
            ProjectPhase projectPhase = new ProjectPhase();
            if (entity == null) return projectPhase;

            MapPortalEntity2Object(entity, projectPhase);
            MapRef(entity.Phase, projectPhase.Phase, entity.PhaseId);

            projectPhase.ProjectId = entity.ProjectId;
            projectPhase.Percentage = entity.Percentage;
            projectPhase.Order = entity.Order;

            return projectPhase;

        }

        public static void MapProjectPhase2LinkProjectPhaseEntity(ProjectPhase obj, ref LinkProjectPhaseEntity entity)
        {
            if (entity == null)
                entity = new LinkProjectPhaseEntity();

            MapPortalObject2Entity(obj, entity);

            entity.ProjectId = obj.ProjectId;
            entity.PhaseId = Ref(obj.Phase);
            entity.Percentage = obj.Percentage;
            entity.Order = obj.Order;

        }

        public static Project MapProjectEntity2Project(ProjectEntity? entity)
        {
            Project project = new Project();
            if (entity == null) return project;

            MapPortalEntity2Object(entity, project);
            MapRef(entity.CurrentPhase, project.CurrentPhase, entity.CurrentPhaseId);

            project.Customer = MapCustomerEntity2Customer(entity.Customer);
            project.Customer.Id = entity.CustomerId;

            foreach (LinkProjectPhaseEntity linkProjectPhaseEntity in entity.Phases ?? Enumerable.Empty<LinkProjectPhaseEntity>())
                project.Phases.Add(MapLinkProjectPhaseEntity2ProjectPhase(linkProjectPhaseEntity));

            foreach (ProjectNoteEntity projectNoteEntity in entity.Notes ?? Enumerable.Empty<ProjectNoteEntity>())
                project.Notes.Add(MapProjectNoteEntity2ProjectNote(projectNoteEntity));

            project.TargetGoLive = entity.TargetGoLiveDate;
            project.ActualGoLive = entity.ActualGoLiveDate;

            return project;

        }

        public static void MapProject2ProjectEntity(Project obj, ref ProjectEntity entity)
        {
            if (entity == null)
                entity = new ProjectEntity();

            MapPortalObject2Entity(obj, entity);

            entity.CustomerId = Ref(obj.Customer);
            entity.CurrentPhaseId = Ref(obj.CurrentPhase);
            entity.TargetGoLiveDate = obj.TargetGoLive;
            entity.ActualGoLiveDate = obj.ActualGoLive;

        }

        public static ProjectNote MapProjectNoteEntity2ProjectNote(ProjectNoteEntity? entity)
        {
            ProjectNote projectNote = new ProjectNote();
            if (entity == null) return projectNote;

            MapPortalEntity2Object(entity, projectNote);

            projectNote.ProjectId = entity.ProjectId;
            projectNote.Note = entity.Note;
            projectNote.CreateTime = entity.CreateTime;

            return projectNote;

        }

        public static void MapProjectNote2ProjectNoteEntity(ProjectNote obj, ref ProjectNoteEntity entity)
        {
            if (entity == null)
                entity = new ProjectNoteEntity();

            MapPortalObject2Entity(obj, entity);

            entity.ProjectId = obj.ProjectId;
            entity.Note = obj.Note;
            entity.CreateTime = obj.CreateTime;

        }

        public static void MapTicket2TicketEntity(Ticket obj, ref TicketEntity entity)
        {
            if (entity == null)
                entity = new TicketEntity();

            MapPortalObject2Entity(obj, entity);

            entity.CustomerId = Ref(obj.Customer);
            entity.IntegrationId = Ref(obj.Integration);
            entity.SeverityId = Ref(obj.Severity);
            entity.StatusId = Ref(obj.Status);
            entity.Reproduce = obj.Reproduce;
            entity.ReportedBy = obj.ReportedBy;
            entity.AssignedTo = obj.AssignedTo;
            entity.CreatedDate = obj.CreatedDate;
            entity.ResolutionDate = obj.ResolutionDate;
            entity.Resolution = obj.Resolution;
            entity.EscalationId = Ref(obj.Escalation);

        }

        public static Ticket MapTicketEntity2Ticket(TicketEntity? entity)
        {
            Ticket ticket = new Ticket();
            if (entity == null) return ticket;

            MapPortalEntity2Object(entity, ticket);

            ticket.Customer = MapCustomerEntity2Customer(entity.Customer);
            ticket.Customer.Id = entity.CustomerId;
            ticket.Integration = MapIntegrationEntity2Integration(entity.Integration);
            ticket.Integration.Id = entity.IntegrationId;

            MapRef(entity.Severity, ticket.Severity, entity.SeverityId);
            MapRef(entity.Status, ticket.Status, entity.StatusId);

            // EscalationId 0 is the DEFAULT sentinel row, not a real escalation.
            ticket.Escalation = entity.EscalationId == 0
                ? new Escalation { Id = 0 }
                : MapEscalationEntity2Escalation(entity.Escalation);

            ticket.Escalation.Id = entity.EscalationId;

            foreach (TicketNoteEntity note in entity.Notes ?? Enumerable.Empty<TicketNoteEntity>())
                ticket.Notes.Add(MapTicketNoteEntity2TicketNote(note));

            ticket.Reproduce = entity.Reproduce;
            ticket.ReportedBy = entity.ReportedBy;
            ticket.AssignedTo = entity.AssignedTo;
            ticket.CreatedDate = entity.CreatedDate;
            ticket.ResolutionDate = entity.ResolutionDate;
            ticket.Resolution = entity.Resolution;

            return ticket;

        }

        public static TicketNote MapTicketNoteEntity2TicketNote(TicketNoteEntity? entity)
        {
            TicketNote ticketNote = new TicketNote();
            if (entity == null) return ticketNote;

            MapPortalEntity2Object(entity, ticketNote);

            ticketNote.TicketId = entity.TicketId;
            ticketNote.Note = entity.Note;
            ticketNote.CreateTime = entity.CreateTime;

            return ticketNote;

        }

        public static void MapTicketNote2TicketNoteEntity(TicketNote obj, ref TicketNoteEntity entity)
        {
            if (entity == null)
                entity = new TicketNoteEntity();
            
            MapPortalObject2Entity(obj, entity);

            entity.TicketId = obj.TicketId;
            entity.Note = obj.Note;
            entity.CreateTime = obj.CreateTime;

        }

        /// <summary>
        /// FK value for a nested reference. A nested PortalObject carries Id -1 until it is
        /// populated (PortalObject ctor), and the datastore has no such row — every table seeds
        /// its "not a real object" row at Id 0. Anything unset therefore collapses to 0.
        /// A positive Id is passed through untouched so a genuinely bad reference still fails
        /// loudly at the FK rather than being silently rewritten to DEFAULT.
        /// </summary>
        private static Int64 Ref(PortalObject? obj) => obj is null || obj.Id < 0 ? 0 : obj.Id;

        /// <summary>
        /// Fills a nested reference: content from the navigation when one was loaded, identity
        /// always from the FK column. A summary query need not Include the navigation, and
        /// without this the mapper cannot tell "not loaded" from "not set" — both would arrive
        /// as Id 0 and a real reference would be reported as the DEFAULT row.
        /// Read-side counterpart to Ref().
        /// </summary>
        private static void MapRef(PortalEntity? entity, PortalObject obj, Int64 foreignKey)
        {
            MapPortalEntity2Object(entity, obj);
            obj.Id = foreignKey;

        }

        public static void MapPortalEntity2Object(PortalEntity? entity, PortalObject obj)
        {
            // Implementation for mapping portal entity to a PortalObject
            if (entity == null) return;

            obj.Id = entity.Id;
            obj.Name = entity.Name;
            obj.Description = entity.Description;
            obj.Deleted = entity.Deleted;
            obj.RowVersion = entity.RowVersion;

        }

        public static void MapPortalObject2Entity(PortalObject obj, PortalEntity entity)
        {
            // Implementation for mapping PortalObject to a portal entity
            entity.Id = obj.Id;
            entity.Name = obj.Name;
            entity.Description = obj.Description;
            entity.Deleted = obj.Deleted;
            entity.RowVersion = obj.RowVersion;

        }

    }

}
