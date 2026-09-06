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
    public class ProjectRepository : IGenericRepository<Project, ProjectList, ProjectEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public ProjectRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<ProjectList>> GetAllAsync(CancellationToken ct)
        {
            List<ProjectList> lstReturn = new List<ProjectList>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllProjects";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<ProjectList>> GetAllActiveAsync(CancellationToken ct)
        {
            List<ProjectList> lstReturn = new List<ProjectList>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllProjects";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectListFromDataReader(result);

            return lstReturn;

        }

        public Task<List<ProjectList>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            throw new NotImplementedException("Parent property not valid for this object.");

        }

        public async Task<Project> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            Project objReturn = new Project();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetProjectById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetProjectFromDataReader(result);

            return objReturn;

        }

        public async Task<Project> GetByNameAsync(string name, CancellationToken ct)
        {
            Project objReturn = new Project();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetProjectByName";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Name", name));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetProjectFromDataReader(result);

            return objReturn;

        }

        public async Task<Int64> CreateAsync(ProjectEntity Project, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateProject";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", Project.Name),
                new SqlParameter("@Description", Project.Description),
                new SqlParameter("@CustomerId", Project.CustomerId),
                new SqlParameter("@CurrentPhase", Project.CurrentPhaseId),
                new SqlParameter("@TargetGoLiveDate", Project.TargetGoLiveDate),
                new SqlParameter("@ActualGoLiveDate", Project.ActualGoLiveDate)

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

        public async Task<Int64> UpdateAsync(ProjectEntity Project, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateProject";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", Project.Id),
                new SqlParameter("@Name", Project.Name),
                new SqlParameter("@Description", Project.Description),
                new SqlParameter("@Deleted", Project.Deleted),
                new SqlParameter("@CustomerId", Project.CustomerId),
                new SqlParameter("@CurrentPhase", Project.CurrentPhaseId),
                new SqlParameter("@TargetGoLiveDate", Project.TargetGoLiveDate),
                new SqlParameter("@ActualGoLiveDate", Project.ActualGoLiveDate),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = Project.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private Project GetProjectFromDataReader(DbDataReader reader)
        {
            Project objReturn = new Project();
            if (reader.HasRows)
            {
                Project newProject = new Project();
                Customer newCustomer = new Customer();
                Industry newIndustry = new Industry();
                Phase newPhase = new Phase();
                List<ProjectNote> lstProjectNotes = new List<ProjectNote>();

                if (reader.Read())
                {

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newProject.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newProject.Name = reader.GetString(reader.GetOrdinal("Name"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newProject.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newProject.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newProject.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("TargetGoLiveDate"))) newProject.TargetGoLive = reader.GetDateTime(reader.GetOrdinal("TargetGoLiveDate"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ActualGoLiveDate"))) newProject.ActualGoLive = reader.GetDateTime(reader.GetOrdinal("ActualGoLiveDate"));

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

                if (reader.NextResult())
                {

                    while (reader.Read())
                    {
                        ProjectNote newNote = new ProjectNote();

                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newNote.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newNote.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newNote.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newNote.RowVersion, 0, 8);
                        if (!reader.IsDBNull(reader.GetOrdinal("ProjectId"))) newNote.ProjectId = reader.GetInt64(reader.GetOrdinal("ProjectId"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Note"))) newNote.Note = reader.GetString(reader.GetOrdinal("Note"));
                        if (!reader.IsDBNull(reader.GetOrdinal("CreateTime"))) newNote.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));

                        if(!newNote.Deleted)
                            lstProjectNotes.Add(newNote);

                    }

                }

                newCustomer.Industry = newIndustry;
                objReturn.Customer = newCustomer;
                objReturn.CurrentPhase = newPhase;
                objReturn.Notes = lstProjectNotes;
                objReturn = newProject;

            }

            return objReturn;

        }

        private List<ProjectList> GetProjectListFromDataReader(DbDataReader reader)
        {
            List<ProjectList> lstReturn = new List<ProjectList>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProjectList newProject = new ProjectList();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newProject.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newProject.Name = reader.GetString(reader.GetOrdinal("Name"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newProject.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newProject.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newProject.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("TargetGoLiveDate"))) newProject.TargetGoLive = reader.GetDateTime(reader.GetOrdinal("TargetGoLiveDate"));
                    if (!reader.IsDBNull(reader.GetOrdinal("ActualGoLiveDate"))) newProject.ActualGoLive = reader.GetDateTime(reader.GetOrdinal("ActualGoLiveDate"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CustomerId"))) newProject.CustomerId = reader.GetInt64(reader.GetOrdinal("CustomerId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CurrentPhase"))) newProject.CurrentPhase = reader.GetInt64(reader.GetOrdinal("CurrentPhase"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CustomerDescription"))) newProject.CustomerDescription = reader.GetString(reader.GetOrdinal("CustomerDescription"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CurrentPhaseDescription"))) newProject.CurrentPhaseDescription = reader.GetString(reader.GetOrdinal("CurrentPhaseDescription"));

                    lstReturn.Add(newProject);

                }

            }

            return lstReturn;

        }

    }

}
