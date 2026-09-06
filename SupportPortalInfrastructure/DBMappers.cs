using System;
using System.Collections.Generic;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure
{
    public class DBMapper
    {
        public static void MapCustomer2CustomerEntity(Customer customer, ref CustomerEntity entity)
        {
            // Implementation for mapping Customer to a customer entity
            if (entity == null) 
                entity = new CustomerEntity();

            MapPortalObject2Entity(customer, entity);
            entity.Name = customer.Name;
            entity.IndustryId = Ref(customer.Industry);
            entity.PrimaryContactName = customer.PrimaryContact;
            entity.PrimaryContactEmail = customer.PrimaryContactEmail;
            entity.TechnicalContactName = customer.TechnicalContact;
            entity.TechnicalContactEmail = customer.TechnicalContactEmail;
            entity.CreatedDate = customer.CreatedDate;

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

        public static void MapIntegration2IntegrationEntity(Integration obj, ref IntegrationEntity entity)
        {
            if (entity == null)
                entity = new IntegrationEntity();

            MapPortalObject2Entity(obj, entity);

            entity.Name = obj.Name;

            entity.IntegrationTypeId = Ref(obj.Type);
            entity.CurrentStatusId = Ref(obj.CurrentStatus);
            entity.CustomerId = Ref(obj.Customer);

            entity.LastSuccessfulSync = obj.LastSuccessfulSync;
            entity.LastFailedSync = obj.LastFailedSync;
            entity.RetryCount = obj.RetryCount;

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

        public static void MapProject2ProjectEntity(Project obj, ref ProjectEntity entity)
        {
            if (entity == null)
                entity = new ProjectEntity();

            MapPortalObject2Entity(obj, entity);

            entity.Name = obj.Name;

            entity.CustomerId = Ref(obj.Customer);
            entity.CurrentPhaseId = Ref(obj.CurrentPhase);
            entity.TargetGoLiveDate = obj.TargetGoLive;
            entity.ActualGoLiveDate = obj.ActualGoLive;

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

        public static void MapPortalObject2Entity(PortalObject obj, PortalEntity entity)
        {
            // Implementation for mapping PortalObject to a portal entity
            entity.Id = obj.Id;
            entity.Description = obj.Description;
            entity.Deleted = obj.Deleted;
            entity.RowVersion = obj.RowVersion;

            // Name belongs to the lookup subtypes. The lookup controllers (Severities,
            // Industries, Phases, SupportStatuses, IntegrationStatuses, IntegrationTypes) map
            // through here with no override, so without this every create and update sends an
            // empty Name to the procedure and the row fails the unique index on it.
            if (obj is PortalLookupObject lookup && entity is PortalLookupEntity lookupEntity)
                lookupEntity.Name = lookup.Name;

        }

    }

}
