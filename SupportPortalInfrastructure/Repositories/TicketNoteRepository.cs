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
    public class TicketNoteRepository : IGenericRepository<TicketNote, TicketNote, TicketNoteEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public TicketNoteRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<TicketNote>> GetAllAsync(CancellationToken ct)
        {
            List<TicketNote> lstReturn = new List<TicketNote>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllTicketNotes";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetTicketNoteListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<TicketNote>> GetAllActiveAsync(CancellationToken ct)
        {
            List<TicketNote> lstReturn = new List<TicketNote>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllTicketNotes";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetTicketNoteListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<TicketNote>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            List<TicketNote> lstReturn = new List<TicketNote>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetTicketNotesByTicketId";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            command.Parameters.Add(new SqlParameter("@TicketId", parentId));

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetTicketNoteListFromDataReader(result);

            return lstReturn;

        }

        public async Task<TicketNote> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            TicketNote objReturn = new TicketNote();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetTicketNoteById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetTicketNoteFromDataReader(result);

            return objReturn;

        }

        public async Task<TicketNote> GetByTicketIdAsync(Int64 id, CancellationToken ct)
        {
            TicketNote objReturn = new TicketNote();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetTicketNoteByTicketId";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetTicketNoteFromDataReader(result);

            return objReturn;

        }

        public Task<TicketNote> GetByNameAsync(string name, CancellationToken ct)
        {
            throw new NotImplementedException("Name property not valid for this object.");

        }

        public async Task<Int64> CreateAsync(TicketNoteEntity TicketNote, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateTicketNote";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@TicketId", TicketNote.TicketId),
                new SqlParameter("@Description", TicketNote.Description),
                new SqlParameter("@Note", TicketNote.Note)

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

        public async Task<Int64> UpdateAsync(TicketNoteEntity TicketNote, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateTicketNote";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", TicketNote.Id),
                new SqlParameter("Deleted", TicketNote.Deleted),
                new SqlParameter("@TicketId", TicketNote.TicketId),
                new SqlParameter("@Description", TicketNote.Description),
                new SqlParameter("@Note", TicketNote.Note),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = TicketNote.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private TicketNote GetTicketNoteFromDataReader(DbDataReader reader)
        {
            TicketNote objReturn = new TicketNote();
            if (reader.HasRows)
            {
                TicketNote newTicketNote = new TicketNote();

                if (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newTicketNote.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newTicketNote.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newTicketNote.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newTicketNote.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("TicketId"))) newTicketNote.TicketId = reader.GetInt64(reader.GetOrdinal("TicketId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Note"))) newTicketNote.Note = reader.GetString(reader.GetOrdinal("Note"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CreateTime"))) newTicketNote.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));

                }

                objReturn = newTicketNote;

            }

            return objReturn;

        }

        private List<TicketNote> GetTicketNoteListFromDataReader(DbDataReader reader)
        {
            List<TicketNote> lstReturn = new List<TicketNote>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    TicketNote newTicketNote = new TicketNote();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newTicketNote.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newTicketNote.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newTicketNote.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newTicketNote.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("TicketId"))) newTicketNote.TicketId = reader.GetInt64(reader.GetOrdinal("TicketId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Note"))) newTicketNote.Note = reader.GetString(reader.GetOrdinal("Note"));
                    if (!reader.IsDBNull(reader.GetOrdinal("CreateTime"))) newTicketNote.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));

                    lstReturn.Add(newTicketNote);

                }

            }

            return lstReturn;

        }

    }

}
