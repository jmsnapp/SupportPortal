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
    public class EscalationRepository : IGenericRepository<Escalation, EscalationListItem, EscalationEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public EscalationRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }
        public async Task<List<EscalationListItem>> GetAllAsync(CancellationToken ct)
        {
            List<EscalationListItem> lstReturn = new List<EscalationListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllEscalations";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetEscalationListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<EscalationListItem>> GetAllActiveAsync(CancellationToken ct)
        {
            List<EscalationListItem> lstReturn = new List<EscalationListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllEscalations";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetEscalationListFromDataReader(result);

            return lstReturn;

        }

        public Task<List<EscalationListItem>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            throw new NotImplementedException("Parent property not valid for this object.");

        }

        public async Task<Escalation> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            Escalation objReturn = new Escalation();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetEscalationById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetEscalationFromDataReader(result);

            return objReturn;

        }

        public Task<Escalation> GetByNameAsync(string name, CancellationToken ct)
        {
            throw new NotImplementedException("Name property not valid for this object.");

        }

        public async Task<Int64> CreateAsync(EscalationEntity Escalation, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateEscalation";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Description", Escalation.Description),
                new SqlParameter("@ProblemSummary", Escalation.ProblemSummary),
                new SqlParameter("@CustomerImpact", Escalation.CustomerImpact),
                new SqlParameter("@RootCause", Escalation.RootCause),
                new SqlParameter("@RecommendedActions", Escalation.RecommendedActions)

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

        public async Task<Int64> UpdateAsync(EscalationEntity Escalation, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateEscalation";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", Escalation.Id),
                new SqlParameter("@Description", Escalation.Description),
                new SqlParameter("@Deleted", Escalation.Deleted),
                new SqlParameter("@ProblemSummary", Escalation.ProblemSummary),
                new SqlParameter("@CustomerImpact", Escalation.CustomerImpact),
                new SqlParameter("@RootCause", Escalation.RootCause),
                new SqlParameter("@RecommendedActions", Escalation.RecommendedActions),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = Escalation.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private Escalation GetEscalationFromDataReader(DbDataReader reader) 
        {
            Escalation objReturn = new Escalation();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) objReturn.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) objReturn.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) objReturn.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, objReturn.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("CreatedDate"))) objReturn.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CustomerImpact"))) objReturn.CustomerImpact = reader.GetString(reader.GetOrdinal("CustomerImpact"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ProblemSummary"))) objReturn.ProblemSummary = reader.GetString(reader.GetOrdinal("ProblemSummary"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RecommendedActions"))) objReturn.RecommendedActions = reader.GetString(reader.GetOrdinal("RecommendedActions"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RootCause"))) objReturn.RootCause = reader.GetString(reader.GetOrdinal("RootCause"));

                }

            }


            return objReturn;
        
        }

        private List<EscalationListItem> GetEscalationListFromDataReader(DbDataReader reader)
        {
            List<EscalationListItem> lstReturn = new List<EscalationListItem>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    EscalationListItem newEscalation = new EscalationListItem();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newEscalation.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newEscalation.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newEscalation.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newEscalation.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("CreatedDate"))) newEscalation.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));

                    lstReturn.Add(newEscalation);

                }

            }

            return lstReturn;

        }

    }

}
