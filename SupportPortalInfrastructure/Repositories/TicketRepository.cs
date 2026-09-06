using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure.Repositories
{
    public class TicketRepository : IGenericRepository<Ticket, TicketListItem, TicketEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public TicketRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<TicketListItem>> GetAllAsync(CancellationToken ct)
        {
            List<TicketListItem> lstReturn = new List<TicketListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllTickets";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetTicketListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<TicketListItem>> GetAllActiveAsync(CancellationToken ct)
        {
            List<TicketListItem> lstReturn = new List<TicketListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllTickets";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetTicketListFromDataReader(result);

            return lstReturn;

        }

        public Task<List<TicketListItem>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            throw new NotImplementedException("Parent property not valid for this object.");

        }

        public async Task<Ticket> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            Ticket objReturn = new Ticket();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetTicketById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetTicketFromDataReader(result);

            return objReturn;

        }

        public Task<Ticket> GetByNameAsync(string name, CancellationToken ct)
        {
            throw new NotImplementedException("Name property not valid for this object.");

        }

        public async Task<Int64> CreateAsync(TicketEntity Ticket, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateTicket";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Description", Ticket.Description),
                new SqlParameter("@CustomerId", Ticket.CustomerId),
                new SqlParameter("@IntegrationId", Ticket.IntegrationId),
                new SqlParameter("@Reproduce", Ticket.Reproduce),
                new SqlParameter("@SeverityId", Ticket.SeverityId),
                new SqlParameter("@StatusId", Ticket.StatusId),
                new SqlParameter("@ReportedBy", Ticket.ReportedBy),
                new SqlParameter("@AssignedTo", Ticket.AssignedTo)

            };

            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);

            }

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            object? result = await command.ExecuteScalarAsync(ct);

            // ExecuteScalar answers null, not DBNull, when the procedure yields no row, and
            // Convert.ToInt64(null) is 0 -- which would read as a successful insert at Id 0.
            return (result is null || result == DBNull.Value) ? 0 : Convert.ToInt64(result);

        }

        public async Task<Int64> UpdateAsync(TicketEntity Ticket, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateTicket";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", Ticket.Id),
                new SqlParameter("@Description", Ticket.Description),
                new SqlParameter("@Deleted", Ticket.Deleted),
                new SqlParameter("@IntegrationId", Ticket.IntegrationId),
                new SqlParameter("@Reproduce", Ticket.Reproduce),
                new SqlParameter("@SeverityId", Ticket.SeverityId),
                new SqlParameter("@StatusId", Ticket.StatusId),
                new SqlParameter("@ReportedBy", Ticket.ReportedBy),
                new SqlParameter("@AssignedTo", Ticket.AssignedTo),
                new SqlParameter("@ResolutionDate", Ticket.AssignedTo),
                new SqlParameter("@Resolution", Ticket.AssignedTo),
                new SqlParameter("@EscalationId", Ticket.AssignedTo),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = Ticket.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
                }

            };

            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);

            }

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            object? result = await command.ExecuteScalarAsync(ct);
            Int64 affected = (result is null || result == DBNull.Value) ? 0 : Convert.ToInt64(result);

            // -1 is the procedure reporting that the row moved on after the caller read it.
            // The API's exception filter turns that into a 409, rather than letting the write
            // silently discard whoever got there first. 0 means the row was found and already
            // held these values, which is a successful no-op, not a conflict.
            if (affected < 0)
                throw new DbUpdateConcurrencyException(
                    "The record was changed by someone else after you loaded it.");

            return affected;

        }

        private Ticket GetTicketFromDataReader(DbDataReader reader)
        {
            Ticket objReturn = new Ticket();
            Severity newSeverity = new Severity();
            SupportStatus newStatus = new SupportStatus();
            Escalation newEscalation = new Escalation();
            Customer newTicketCustomer = new Customer();
            Industry newTicketIndustry = new Industry();
            Integration newIntegration = new Integration();
            IntegrationStatus newIntegrationStatus = new IntegrationStatus();
            IntegrationType newIntegrationType = new IntegrationType();
            Customer newIntegrationCustomer = new Customer();
            Industry newIntegrationIndustry = new Industry();
            List<TicketNote> lstTicketNotes = new List<TicketNote>();

            if (reader.HasRows)
            {

                if (reader.Read())
                {

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) objReturn.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) objReturn.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) objReturn.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, objReturn.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("Reproduce"))) objReturn.Reproduce = reader.GetString(reader.GetOrdinal("Reproduce"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ReportedBy"))) objReturn.ReportedBy = reader.GetString(reader.GetOrdinal("ReportedBy"));
                    if (!reader.IsDBNull(reader.GetOrdinal("AssignedTo"))) objReturn.AssignedTo = reader.GetString(reader.GetOrdinal("AssignedTo"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CreatedDate"))) objReturn.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ResolutionDate"))) objReturn.ResolutionDate = reader.GetDateTime(reader.GetOrdinal("ResolutionDate"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Resolution"))) objReturn.Description = reader.GetString(reader.GetOrdinal("Resolution"));

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newSeverity.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newSeverity.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newSeverity.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newSeverity.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newSeverity.RowVersion, 0, 8);

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newStatus.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newStatus.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newStatus.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newStatus.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newStatus.RowVersion, 0, 8);

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newEscalation.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newEscalation.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newEscalation.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newEscalation.RowVersion, 0, 8);
                        if (!reader.IsDBNull(reader.GetOrdinal("CreatedDate"))) newEscalation.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                        if (!reader.IsDBNull(reader.GetOrdinal("CustomerImpact"))) newEscalation.CustomerImpact = reader.GetString(reader.GetOrdinal("CustomerImpact"));
                        if (!reader.IsDBNull(reader.GetOrdinal("ProblemSummary"))) newEscalation.ProblemSummary = reader.GetString(reader.GetOrdinal("ProblemSummary"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RecommendedActions"))) newEscalation.RecommendedActions = reader.GetString(reader.GetOrdinal("RecommendedActions"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RootCause"))) newEscalation.RootCause = reader.GetString(reader.GetOrdinal("RootCause"));

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newTicketCustomer.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newTicketCustomer.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newTicketCustomer.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newTicketCustomer.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newTicketCustomer.RowVersion, 0, 8);
                        if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactName"))) newTicketCustomer.PrimaryContact = reader.GetString(reader.GetOrdinal("PrimaryContactName"));
                        if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactEmail"))) newTicketCustomer.PrimaryContactEmail = reader.GetString(reader.GetOrdinal("PrimaryContactEmail"));
                        if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactName"))) newTicketCustomer.TechnicalContact = reader.GetString(reader.GetOrdinal("TechnicalContactName"));
                        if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactEmail"))) newTicketCustomer.TechnicalContactEmail = reader.GetString(reader.GetOrdinal("TechnicalContactEmail"));

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newTicketIndustry.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newTicketIndustry.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newTicketIndustry.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newTicketIndustry.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newTicketIndustry.RowVersion, 0, 8);

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIntegration.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIntegration.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIntegration.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIntegration.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIntegration.RowVersion, 0, 8);
                        if (!reader.IsDBNull(reader.GetOrdinal("LastSuccessfulSync"))) newIntegration.LastSuccessfulSync = reader.GetDateTime(reader.GetOrdinal("LastSuccessfulSync"));
                        if (!reader.IsDBNull(reader.GetOrdinal("LastFailedSync"))) newIntegration.LastFailedSync = reader.GetDateTime(reader.GetOrdinal("LastFailedSync"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RetryCount"))) newIntegration.RetryCount = reader.GetInt32(reader.GetOrdinal("RetryCount"));

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIntegrationStatus.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIntegrationStatus.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIntegrationStatus.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIntegrationStatus.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIntegrationStatus.RowVersion, 0, 8);

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIntegrationType.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIntegrationType.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIntegrationType.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIntegrationType.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIntegrationType.RowVersion, 0, 8);

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIntegrationCustomer.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIntegrationCustomer.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIntegrationCustomer.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIntegrationCustomer.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIntegrationCustomer.RowVersion, 0, 8);
                        if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactName"))) newIntegrationCustomer.PrimaryContact = reader.GetString(reader.GetOrdinal("PrimaryContactName"));
                        if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactEmail"))) newIntegrationCustomer.PrimaryContactEmail = reader.GetString(reader.GetOrdinal("PrimaryContactEmail"));
                        if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactName"))) newIntegrationCustomer.TechnicalContact = reader.GetString(reader.GetOrdinal("TechnicalContactName"));
                        if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactEmail"))) newIntegrationCustomer.TechnicalContactEmail = reader.GetString(reader.GetOrdinal("TechnicalContactEmail"));

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIntegrationIndustry.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIntegrationIndustry.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIntegrationIndustry.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIntegrationIndustry.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIntegrationIndustry.RowVersion, 0, 8);

                    }

                }

                if (reader.NextResult())
                {

                    while (reader.Read())
                    {
                        TicketNote newNote = new TicketNote();

                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newNote.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newNote.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newNote.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newNote.RowVersion, 0, 8);
                        if (!reader.IsDBNull(reader.GetOrdinal("TicketId"))) newNote.TicketId = reader.GetInt64(reader.GetOrdinal("TicketId"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Note"))) newNote.Note = reader.GetString(reader.GetOrdinal("Note"));
                        if (!reader.IsDBNull(reader.GetOrdinal("CreateTime"))) newNote.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));

                        if (!newNote.Deleted)
                            lstTicketNotes.Add(newNote);

                    }

                }

            }

            objReturn.Severity = newSeverity;
            objReturn.Status = newStatus;
            newTicketCustomer.Industry = newTicketIndustry;
            objReturn.Customer = newTicketCustomer;
            newIntegrationCustomer.Industry = newIntegrationIndustry;
            newIntegration.Customer = newIntegrationCustomer;
            newIntegration.CurrentStatus = newIntegrationStatus;
            newIntegration.Type = newIntegrationType;
            objReturn.Integration = newIntegration;
            objReturn.Escalation = newEscalation;
            objReturn.Notes = lstTicketNotes;

            return objReturn;

        }

        private List<TicketListItem> GetTicketListFromDataReader(DbDataReader reader)
        {
            List<TicketListItem> lstReturn = new List<TicketListItem>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    TicketListItem newTicket = new TicketListItem();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newTicket.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newTicket.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newTicket.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newTicket.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("CustomerId"))) newTicket.CustomerId = reader.GetInt64(reader.GetOrdinal("CustomerId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("IntegrationId"))) newTicket.IntegrationId = reader.GetInt64(reader.GetOrdinal("IntegrationId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("SeverityId"))) newTicket.SeverityId = reader.GetInt64(reader.GetOrdinal("SeverityId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("StatusId"))) newTicket.StatusId = reader.GetInt64(reader.GetOrdinal("StatusId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("EscalationId"))) newTicket.EscalationId = reader.GetInt64(reader.GetOrdinal("EscalationId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CreatedDate"))) newTicket.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ResolutionDate"))) newTicket.ResolutionDate = reader.GetDateTime(reader.GetOrdinal("ResolutionDate"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Reproduce"))) newTicket.Reproduce = reader.GetString(reader.GetOrdinal("Reproduce"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ReportedBy"))) newTicket.ReportedBy = reader.GetString(reader.GetOrdinal("ReportedBy"));
                    if (!reader.IsDBNull(reader.GetOrdinal("AssignedTo"))) newTicket.Description = reader.GetString(reader.GetOrdinal("AssignedTo"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Resolution"))) newTicket.Description = reader.GetString(reader.GetOrdinal("Resolution"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CustomerDescription"))) newTicket.CustomerDescription = reader.GetString(reader.GetOrdinal("CustomerDescription"));
                    if (!reader.IsDBNull(reader.GetOrdinal("IntegrationDescription"))) newTicket.IntegrationDescription = reader.GetString(reader.GetOrdinal("IntegrationDescription"));
                    if (!reader.IsDBNull(reader.GetOrdinal("SeverityDescription"))) newTicket.SeverityDescription = reader.GetString(reader.GetOrdinal("SeverityDescription"));
                    if (!reader.IsDBNull(reader.GetOrdinal("StatusDescription"))) newTicket.StatusDescription = reader.GetString(reader.GetOrdinal("StatusDescription"));
                    if (!reader.IsDBNull(reader.GetOrdinal("EscalationDescription"))) newTicket.EscalationDescription = reader.GetString(reader.GetOrdinal("EscalationDescription"));

                    lstReturn.Add(newTicket);

                }

            }

            return lstReturn;

        }

    }

}
