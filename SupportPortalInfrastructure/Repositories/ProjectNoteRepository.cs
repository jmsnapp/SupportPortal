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
    public class ProjectNoteRepository : IGenericRepository<ProjectNote, ProjectNote, ProjectNoteEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public ProjectNoteRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<ProjectNote>> GetAllAsync(CancellationToken ct)
        {
            List<ProjectNote> lstReturn = new List<ProjectNote>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllProjectNotes";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectNoteListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<ProjectNote>> GetAllActiveAsync(CancellationToken ct)
        {
            List<ProjectNote> lstReturn = new List<ProjectNote>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllProjectNotes";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectNoteListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<ProjectNote>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            List<ProjectNote> lstReturn = new List<ProjectNote>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetProjectNotesByProjectId";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            command.Parameters.Add(new SqlParameter("@ProjectId", parentId));

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetProjectNoteListFromDataReader(result);

            return lstReturn;

        }

        public async Task<ProjectNote> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            ProjectNote objReturn = new ProjectNote();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetProjectNoteById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetProjectNoteFromDataReader(result);

            return objReturn;

        }

        public Task<ProjectNote> GetByNameAsync(string name, CancellationToken ct)
        {
            throw new NotImplementedException("Name property not valid for this object.");

        }

        public async Task<ProjectNote> GetByProjectIdAsync(Int64 id, CancellationToken ct)
        {
            ProjectNote objReturn = new ProjectNote();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetProjectNoteByProjectId";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetProjectNoteFromDataReader(result);

            return objReturn;

        }

        public async Task<Int64> CreateAsync(ProjectNoteEntity ProjectNote, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateProjectNote";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", ProjectNote.ProjectId),
                new SqlParameter("@Description", ProjectNote.Description),
                new SqlParameter("@Note", ProjectNote.Note)

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

        public async Task<Int64> UpdateAsync(ProjectNoteEntity ProjectNote, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateProjectNote";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", ProjectNote.Id),
                new SqlParameter("Deleted", ProjectNote.Deleted),
                new SqlParameter("@ProjectId", ProjectNote.ProjectId),
                new SqlParameter("@Description", ProjectNote.Description),
                new SqlParameter("@Note", ProjectNote.Note),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = ProjectNote.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private ProjectNote GetProjectNoteFromDataReader(DbDataReader reader)
        {
            ProjectNote objReturn = new ProjectNote();
            if (reader.HasRows)
            {
                ProjectNote newProjectNote = new ProjectNote();

                if (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newProjectNote.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newProjectNote.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newProjectNote.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newProjectNote.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("ProjectId"))) newProjectNote.ProjectId = reader.GetInt64(reader.GetOrdinal("ProjectId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Note"))) newProjectNote.Note = reader.GetString(reader.GetOrdinal("Note"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CreateTime"))) newProjectNote.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));

                }

                objReturn = newProjectNote;

            }

            return objReturn;

        }

        private List<ProjectNote> GetProjectNoteListFromDataReader(DbDataReader reader)
        {
            List<ProjectNote> lstReturn = new List<ProjectNote>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProjectNote newProjectNote = new ProjectNote();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newProjectNote.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newProjectNote.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newProjectNote.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newProjectNote.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("ProjectId"))) newProjectNote.ProjectId = reader.GetInt64(reader.GetOrdinal("ProjectId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Note"))) newProjectNote.Note = reader.GetString(reader.GetOrdinal("Note"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CreateTime"))) newProjectNote.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));

                    lstReturn.Add(newProjectNote);

                }

            }

            return lstReturn;

        }

    }

}
