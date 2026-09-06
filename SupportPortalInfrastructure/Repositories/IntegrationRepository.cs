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
    public class IntegrationRepository : IGenericRepository<Integration, IntegrationListItem, IntegrationEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public IntegrationRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<IntegrationListItem>> GetAllAsync(CancellationToken ct)
        {
            List<IntegrationListItem> lstReturn = new List<IntegrationListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllIntegrations";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetIntegrationListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<IntegrationListItem>> GetAllActiveAsync(CancellationToken ct)
        {
            List<IntegrationListItem> lstReturn = new List<IntegrationListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllIntegrations";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetIntegrationListFromDataReader(result);

            return lstReturn;

        }

        public Task<List<IntegrationListItem>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            throw new NotImplementedException("Parent property not valid for this object.");

        }

        public async Task<Integration> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            Integration objReturn = new Integration();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetIntegrationById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetIntegrationFromDataReader(result);

            return objReturn;

        }

        public async Task<Integration> GetByNameAsync(string name, CancellationToken ct)
        {
            Integration objReturn = new Integration();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetIntegrationByName";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Name", name));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetIntegrationFromDataReader(result);

            return objReturn;

        }

        public async Task<Int64> CreateAsync(IntegrationEntity entity, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateIntegration";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("Name", entity.Name),
                new SqlParameter("Description", entity.Description),
                new SqlParameter("LastSuccessfulSync", entity.LastSuccessfulSync),
                new SqlParameter("LastFailedSync", entity.LastFailedSync),
                new SqlParameter("RetryCount", entity.RetryCount)

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

        public async Task<Int64> UpdateAsync(IntegrationEntity entity, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateIntegration";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", entity.Id),
                new SqlParameter("Name", entity.Name),
                new SqlParameter("Description", entity.Description),
                new SqlParameter("Deleted", entity.Deleted),
                new SqlParameter("LastSuccessfulSync", entity.LastSuccessfulSync),
                new SqlParameter("LastFailedSync", entity.LastFailedSync),
                new SqlParameter("RetryCount", entity.RetryCount),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = entity.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private Integration GetIntegrationFromDataReader(DbDataReader reader)
        {
            Integration objReturn = new Integration();
            Customer newCustomer = new Customer();
            Industry newIndustry = new Industry();
            IntegrationType newIntegrationType = new IntegrationType();
            IntegrationStatus newIntegrationStatus = new IntegrationStatus();

            if (reader.HasRows)
            {
                if (reader.Read())
                {

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) objReturn.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Name"))) objReturn.Name = reader.GetString(reader.GetOrdinal("Name"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) objReturn.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) objReturn.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, objReturn.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("LastSuccessfulSync"))) objReturn.LastSuccessfulSync = reader.GetDateTime(reader.GetOrdinal("LastSuccessfulSync"));
                    if (!reader.IsDBNull(reader.GetOrdinal("LastFailedSync"))) objReturn.LastFailedSync = reader.GetDateTime(reader.GetOrdinal("LastFailedSync"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RetryCount"))) objReturn.RetryCount = reader.GetInt32(reader.GetOrdinal("RetryCount"));

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

            objReturn.CurrentStatus = newIntegrationStatus;
            objReturn.Type = newIntegrationType;
            newCustomer.Industry = newIndustry;
            objReturn.Customer = newCustomer;

            return objReturn;

        }

        private List<IntegrationListItem> GetIntegrationListFromDataReader(DbDataReader reader)
        {
            List<IntegrationListItem> lstReturn = new List<IntegrationListItem>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    IntegrationListItem newIntegration = new IntegrationListItem();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIntegration.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIntegration.Name = reader.GetString(reader.GetOrdinal("Name"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIntegration.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIntegration.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIntegration.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("LastSuccessfulSync"))) newIntegration.LastSuccessfulSync = reader.GetDateTime(reader.GetOrdinal("LastSuccessfulSync"));
                    if (!reader.IsDBNull(reader.GetOrdinal("LastFailedSync"))) newIntegration.LastFailedSync = reader.GetDateTime(reader.GetOrdinal("LastFailedSync"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RetryCount"))) newIntegration.RetryCount = reader.GetInt32(reader.GetOrdinal("RetryCount"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CustomerId"))) newIntegration.Id = reader.GetInt64(reader.GetOrdinal("CustomerId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("IntegrationTypeId"))) newIntegration.Id = reader.GetInt64(reader.GetOrdinal("IntegrationTypeId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CurrentStatusId"))) newIntegration.Id = reader.GetInt64(reader.GetOrdinal("CurrentStatusId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CustomerDescription"))) newIntegration.CustomerDescription = reader.GetString(reader.GetOrdinal("CustomerDescription"));
                    if (!reader.IsDBNull(reader.GetOrdinal("IntegrationTypeDescription"))) newIntegration.IntegrationTypeDescription = reader.GetString(reader.GetOrdinal("IntegrationTypeDescription"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CurrentStatusDescription"))) newIntegration.CurrentStatusDescription = reader.GetString(reader.GetOrdinal("CurrentStatusDescription"));

                    lstReturn.Add(newIntegration);

                }

            }

            return lstReturn;

        }

    }

}
