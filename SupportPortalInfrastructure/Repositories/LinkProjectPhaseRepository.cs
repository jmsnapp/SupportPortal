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
    public class ProjectPhaseRepository : IGenericRepository<ProjectPhase, ProjectPhaseListItem, LinkProjectPhaseEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public ProjectPhaseRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<ProjectPhaseListItem>> GetAllAsync(CancellationToken ct)
        {
            List<ProjectPhaseListItem> lstReturn = new List<ProjectPhaseListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllLinkProjectPhases";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectPhaseListItemsFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<ProjectPhaseListItem>> GetAllActiveAsync(CancellationToken ct)
        {
            List<ProjectPhaseListItem> lstReturn = new List<ProjectPhaseListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllLinkProjectPhases";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectPhaseListItemsFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<ProjectPhaseListItem>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            List<ProjectPhaseListItem> lstReturn = new List<ProjectPhaseListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetProjectPhasesByProjectId";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            command.Parameters.Add(new SqlParameter("@ProjectId", parentId));

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectPhaseListItemsFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<ProjectPhaseListItem>> GetProjectPhasesByProjectAsync(CancellationToken ct)
        {
            List<ProjectPhaseListItem> lstReturn = new List<ProjectPhaseListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetProjectPhasesByProjectId";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectPhaseListItemsFromDataReader(result);

            return lstReturn;

        }

        public async Task<ProjectPhase> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            ProjectPhase objReturn = new ProjectPhase();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetProjectPhaseById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetProjectPhaseFromDataReader(result);

            return objReturn;

        }

        public Task<ProjectPhase> GetByNameAsync(string name, CancellationToken ct)
        {
            throw new NotImplementedException("Name property not valid for this object.");

        }

        public async Task<Int64> CreateAsync(LinkProjectPhaseEntity ProjectPhase, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateLinkProjectPhase";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Description", ProjectPhase.Description),
                new SqlParameter("@ProjectId ", ProjectPhase.ProjectId),
                new SqlParameter("@PhaseId", ProjectPhase.PhaseId),
                new SqlParameter("@Percentage", ProjectPhase.Percentage),
                new SqlParameter("@Order", ProjectPhase.Order)

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

        public async Task<Int64> UpdateAsync(LinkProjectPhaseEntity ProjectPhase, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateLinkProjectPhase";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", ProjectPhase.Id),
                new SqlParameter("@Description", ProjectPhase.Description),
                new SqlParameter("@Deleted", ProjectPhase.Deleted),
                new SqlParameter("@ProjectId ", ProjectPhase.ProjectId),
                new SqlParameter("@PhaseId", ProjectPhase.PhaseId),
                new SqlParameter("@Percentage", ProjectPhase.Percentage),
                new SqlParameter("@Order", ProjectPhase.Order),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = ProjectPhase.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private ProjectPhase GetProjectPhaseFromDataReader(DbDataReader reader)
        {
            ProjectPhase objReturn = new ProjectPhase();
            Phase newPhase = new Phase();

            if (reader.HasRows)
            {
                if (reader.Read())
                {

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) objReturn.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) objReturn.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) objReturn.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, objReturn.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("ProjectId"))) objReturn.ProjectId = reader.GetInt64(reader.GetOrdinal("ProjectId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Percentage"))) objReturn.Percentage = reader.GetDecimal(reader.GetOrdinal("Percentage"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Order"))) objReturn.Order = reader.GetInt32(reader.GetOrdinal("Order"));

                }

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newPhase.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newPhase.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newPhase.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newPhase.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newPhase.RowVersion, 0, 8);

                    }

                }

            }

            objReturn.Phase = newPhase;

            return objReturn;

        }

        private List<ProjectPhaseListItem> GetProjectPhaseListItemsFromDataReader(DbDataReader reader)
        {
            List<ProjectPhaseListItem> lstReturn = new List<ProjectPhaseListItem>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProjectPhaseListItem newProjectPhase = new ProjectPhaseListItem();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newProjectPhase.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newProjectPhase.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newProjectPhase.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newProjectPhase.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("ProjectId"))) newProjectPhase.ProjectId = reader.GetInt64(reader.GetOrdinal("ProjectId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Percentage"))) newProjectPhase.Percentage = reader.GetDecimal(reader.GetOrdinal("Percentage"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Order"))) newProjectPhase.Order = reader.GetInt32(reader.GetOrdinal("Order"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ProjectDescription"))) newProjectPhase.ProjectDescription = reader.GetString(reader.GetOrdinal("ProjectDescription"));
                    if (!reader.IsDBNull(reader.GetOrdinal("PhaseDescription"))) newProjectPhase.PhaseDescription = reader.GetString(reader.GetOrdinal("PhaseDescription"));

                    lstReturn.Add(newProjectPhase);

                }

            }

            return lstReturn;

        }

    }

}
