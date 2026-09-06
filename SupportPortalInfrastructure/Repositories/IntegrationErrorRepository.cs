using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalInfrastructure.Repositories
{
    public class IntegrationErrorRepository : IGenericRepository<IntegrationError, IntegrationErrorListItem, IntegrationErrorEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public IntegrationErrorRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<IntegrationErrorListItem>> GetAllAsync(CancellationToken ct)
        {
            List<IntegrationErrorListItem> lstReturn = new List<IntegrationErrorListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllIntegrationErrors";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetIntegrationErrorListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<IntegrationErrorListItem>> GetAllActiveAsync(CancellationToken ct)
        {
            List<IntegrationErrorListItem> lstReturn = new List<IntegrationErrorListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllIntegrationErrors";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetIntegrationErrorListFromDataReader(result);

            return lstReturn;

        }

        public Task<List<IntegrationErrorListItem>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            throw new NotImplementedException("Parent property not valid for this object.");

        }

        public async Task<IntegrationError> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            IntegrationError objReturn = new IntegrationError();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetIntegrationErrorById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetIntegrationErrorFromDataReader(result);

            return objReturn;

        }

        public Task<IntegrationError> GetByNameAsync(string name, CancellationToken ct)
        {
            throw new NotImplementedException("Name property not valid for this object.");

        }

        public async Task<Int64> CreateAsync(IntegrationErrorEntity IntegrationError, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateIntegrationError";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Description", IntegrationError.Description),
                new SqlParameter("@IntegrationId", IntegrationError.IntegrationId),
                new SqlParameter("@ErrorMessage", IntegrationError.ErrorMessage),
                new SqlParameter("@StackTrace", IntegrationError.StackTrace),
                new SqlParameter("@ErrorTime", IntegrationError.ErrorTime)

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

        public async Task<Int64> UpdateAsync(IntegrationErrorEntity IntegrationError, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateIntegrationError";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", IntegrationError.Id),
                new SqlParameter("@Description", IntegrationError.Description),
                new SqlParameter("@Deleted", IntegrationError.Deleted),
                new SqlParameter("@IntegrationId", IntegrationError.IntegrationId),
                new SqlParameter("@ErrorMessage", IntegrationError.ErrorMessage),
                new SqlParameter("@StackTrace", IntegrationError.StackTrace),
                new SqlParameter("@ErrorTime", IntegrationError.ErrorTime),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = IntegrationError.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private IntegrationError GetIntegrationErrorFromDataReader(DbDataReader reader)
        {
            IntegrationError objReturn = new IntegrationError();
            Integration newIntegration = new Integration();
            Customer newCustomer = new Customer();
            Industry newIndustry = new Industry();
            IntegrationType newIntegrationType = new IntegrationType();
            IntegrationStatus newIntegrationStatus = new IntegrationStatus();

            if (reader.HasRows)
            {
                if (reader.Read())
                {

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) objReturn.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) objReturn.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) objReturn.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, objReturn.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("ErrorMessage"))) objReturn.ErrorMessage = reader.GetString(reader.GetOrdinal("ErrorMessage"));
                    if (!reader.IsDBNull(reader.GetOrdinal("StackTrace"))) objReturn.StackTrace = reader.GetString(reader.GetOrdinal("StackTrace"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ErrorTime"))) objReturn.ErrorTime = reader.GetDateTime(reader.GetOrdinal("ErrorTime"));

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
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newCustomer.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newCustomer.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newCustomer.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newCustomer.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newCustomer.RowVersion, 0, 8);
                        if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactName"))) newCustomer.PrimaryContact = reader.GetString(reader.GetOrdinal("PrimaryContactName"));
                        if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactEmail"))) newCustomer.PrimaryContactEmail = reader.GetString(reader.GetOrdinal("PrimaryContactEmail"));
                        if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactName"))) newCustomer.TechnicalContact = reader.GetString(reader.GetOrdinal("TechnicalContactName"));
                        if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactEmail"))) newCustomer.TechnicalContactEmail = reader.GetString(reader.GetOrdinal("TechnicalContactEmail"));

                    }

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIndustry.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIndustry.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIndustry.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIndustry.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIndustry.RowVersion, 0, 8);

                    }

                }

            }

            newIntegration.CurrentStatus = newIntegrationStatus;
            newIntegration.Type = newIntegrationType;
            newCustomer.Industry = newIndustry;
            newIntegration.Customer = newCustomer;
            objReturn.Integration = newIntegration;

            return objReturn;

        }

        private List<IntegrationErrorListItem> GetIntegrationErrorListFromDataReader(DbDataReader reader)
        {
            List<IntegrationErrorListItem> lstReturn = new List<IntegrationErrorListItem>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    IntegrationErrorListItem newItem = new IntegrationErrorListItem();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newItem.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newItem.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newItem.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newItem.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("IntegrationId"))) newItem.IntegrationId = reader.GetInt64(reader.GetOrdinal("IntegrationId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ErrorMessage"))) newItem.ErrorMessage = reader.GetString(reader.GetOrdinal("ErrorMessage"));
                    if (!reader.IsDBNull(reader.GetOrdinal("StackTrace"))) newItem.StackTrace = reader.GetString(reader.GetOrdinal("StackTrace"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ErrorTime"))) newItem.ErrorTime = reader.GetDateTime(reader.GetOrdinal("ErrorTime"));
                    if (!reader.IsDBNull(reader.GetOrdinal("IntegrationDescription"))) newItem.IntegrationDescription = reader.GetString(reader.GetOrdinal("IntegrationDescription"));

                    lstReturn.Add(newItem);

                }

            }

            return lstReturn;

        }

    }

}
