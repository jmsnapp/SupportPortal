using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SupportPortalInfrastructure.Repositories
{
    public class PhaseRepository : IGenericRepository<Phase, Phase, PhaseEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public PhaseRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<Phase>> GetAllAsync(CancellationToken ct)
        {
            List<Phase> lstReturn = new List<Phase>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllPhases";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetPhaseListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<Phase>> GetAllActiveAsync(CancellationToken ct)
        {
            List<Phase> lstReturn = new List<Phase>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllPhases";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetPhaseListFromDataReader(result);

            return lstReturn;

        }

        public Task<List<Phase>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            throw new NotImplementedException("Parent property not valid for this object.");

        }

        public async Task<Phase> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            Phase objReturn = new Phase();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetPhaseById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetPhaseFromDataReader(result);

            return objReturn;

        }

        public async Task<Phase> GetByNameAsync(string name, CancellationToken ct)
        {
            Phase objReturn = new Phase();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetPhaseByName";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Name", name));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetPhaseFromDataReader(result);

            return objReturn;

        }

        public async Task<Int64> CreateAsync(PhaseEntity industry, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreatePhase";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", industry.Name),
                new SqlParameter("@Description", industry.Description)

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

        public async Task<Int64> UpdateAsync(PhaseEntity industry, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdatePhase";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", industry.Id),
                new SqlParameter("@Name", industry.Name),
                new SqlParameter("@Description", industry.Description),
                new SqlParameter("@Deleted", industry.Deleted),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = industry.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private Phase GetPhaseFromDataReader(DbDataReader reader)
        {
            Phase objReturn = new Phase();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) objReturn.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Name"))) objReturn.Name = reader.GetString(reader.GetOrdinal("Name"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) objReturn.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) objReturn.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, objReturn.RowVersion, 0, 8);

                }

            }

            return objReturn;

        }

        private List<Phase> GetPhaseListFromDataReader(DbDataReader reader)
        {
            List<Phase> lstReturn = new List<Phase>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Phase newIndustry = new Phase();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIndustry.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIndustry.Name = reader.GetString(reader.GetOrdinal("Name"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIndustry.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIndustry.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIndustry.RowVersion, 0, 8);

                    lstReturn.Add(newIndustry);

                }

            }

            return lstReturn;

        }

    }

}
