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
    public class CustomerRepository : IGenericRepository<Customer, CustomerListItem, CustomerEntity>
    {
        protected readonly SupportPortalDBContext _context;

        /// <summary>
        /// Create a new instance of the generic repository.
        /// </summary>
        /// <param name="context">The EF DB context.</param>
        public CustomerRepository(SupportPortalDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<List<CustomerListItem>> GetAllAsync(CancellationToken ct)
        {
            List<CustomerListItem> lstReturn = new List<CustomerListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllCustomers";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetCustomerListFromDataReader(result);

            return lstReturn;

        }

        public async Task<List<CustomerListItem>> GetAllActiveAsync(CancellationToken ct)
        {
            List<CustomerListItem> lstReturn = new List<CustomerListItem>();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetAllCustomers";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            // Execute reader
            var result = await command.ExecuteReaderAsync(ct);

            lstReturn = GetCustomerListFromDataReader(result);

            return lstReturn;

        }

        public Task<List<CustomerListItem>> GetByParentIdAsync(Int64 parentId, CancellationToken ct)
        {
            throw new NotImplementedException("Parent property not valid for this object.");

        }

        public async Task<Customer> GetByIdAsync(Int64 id, CancellationToken ct)
        {
            Customer objReturn = new Customer();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetCustomerById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetCustomerFromDataReader(result);

            return objReturn;

        }

        public async Task<Customer> GetByNameAsync(string name, CancellationToken ct)
        {
            Customer objReturn = new Customer();

            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspGetCustomerByName";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Name", name));

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            var result = await command.ExecuteReaderAsync(ct);

            objReturn = GetCustomerFromDataReader(result);

            return objReturn;

        }

        public async Task<Int64> CreateAsync(CustomerEntity Customer, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspCreateCustomer";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", Customer.Name),
                new SqlParameter("@Description", Customer.Description),
                new SqlParameter("@IndustryId", Customer.IndustryId),
                new SqlParameter("@PrimaryContactName", Customer.PrimaryContactName),
                new SqlParameter("@PrimaryContactEmail", Customer.PrimaryContactEmail),
                new SqlParameter("@TechnicalContactName", Customer.TechnicalContactName),
                new SqlParameter("@TechnicalContactEmail", Customer.TechnicalContactEmail)
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

        public async Task<Int64> UpdateAsync(CustomerEntity Customer, CancellationToken ct)
        {
            // Ensure connection is open
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "dbo.uspUpdateCustomer";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("Id", Customer.Id),
                new SqlParameter("@Name", Customer.Name),
                new SqlParameter("@Description", Customer.Description),
                new SqlParameter("@Deleted", Customer.Deleted),
                new SqlParameter("@IndustryId", Customer.IndustryId),
                new SqlParameter("@PrimaryContactName", Customer.PrimaryContactName),
                new SqlParameter("@PrimaryContactEmail", Customer.PrimaryContactEmail),
                new SqlParameter("@TechnicalContactName", Customer.TechnicalContactName),
                new SqlParameter("@TechnicalContactEmail", Customer.TechnicalContactEmail),

                // The version the caller believes the row is at. Empty means they stated no
                // expectation, and the procedure then leaves the write unguarded.
                new SqlParameter("@RowVersion", SqlDbType.Binary, 8)
                {
                    // Typed explicitly: SqlClient infers NVarChar from a bare DBNull, and the
                    // procedure declares BINARY(8), which SQL Server will not convert implicitly.
                    Value = Customer.RowVersion is { Length: > 0 } expected ? expected : (object)DBNull.Value
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

        private Customer GetCustomerFromDataReader(DbDataReader reader) 
        {
            Customer objReturn = new Customer();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) objReturn.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Name"))) objReturn.Name = reader.GetString(reader.GetOrdinal("Name"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) objReturn.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) objReturn.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, objReturn.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactName"))) objReturn.PrimaryContact = reader.GetString(reader.GetOrdinal("PrimaryContactName"));
                    if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactEmail"))) objReturn.PrimaryContactEmail = reader.GetString(reader.GetOrdinal("PrimaryContactEmail"));
                    if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactName"))) objReturn.TechnicalContact = reader.GetString(reader.GetOrdinal("TechnicalContactName"));
                    if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactEmail"))) objReturn.TechnicalContactEmail = reader.GetString(reader.GetOrdinal("TechnicalContactEmail"));

                }

                Industry newIndustryModel = new Industry();

                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newIndustryModel.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newIndustryModel.Name = reader.GetString(reader.GetOrdinal("Name"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newIndustryModel.Description = reader.GetString(reader.GetOrdinal("Description"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newIndustryModel.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                        if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newIndustryModel.RowVersion, 0, 8);

                    }

                }

                objReturn.Industry = newIndustryModel;

            }

            return objReturn;
        
        }

        private List<CustomerListItem> GetCustomerListFromDataReader(DbDataReader reader)
        {
            List<CustomerListItem> lstReturn = new List<CustomerListItem>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CustomerListItem newCustomerItem = new CustomerListItem();

                    if (!reader.IsDBNull(reader.GetOrdinal("Id"))) newCustomerItem.Id = reader.GetInt64(reader.GetOrdinal("Id"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Name"))) newCustomerItem.Name = reader.GetString(reader.GetOrdinal("Name"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Description"))) newCustomerItem.Description = reader.GetString(reader.GetOrdinal("Description"));
                    if (!reader.IsDBNull(reader.GetOrdinal("Deleted"))) newCustomerItem.Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted"));
                    if (!reader.IsDBNull(reader.GetOrdinal("RowVersion"))) reader.GetBytes(reader.GetOrdinal("RowVersion"), 0, newCustomerItem.RowVersion, 0, 8);
                    if (!reader.IsDBNull(reader.GetOrdinal("IndustryId"))) newCustomerItem.Id = reader.GetInt64(reader.GetOrdinal("IndustryId"));
                    if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactName"))) newCustomerItem.PrimaryContactName = reader.GetString(reader.GetOrdinal("PrimaryContactName"));
                    if (!reader.IsDBNull(reader.GetOrdinal("PrimaryContactEmail"))) newCustomerItem.PrimaryContactEmail = reader.GetString(reader.GetOrdinal("PrimaryContactEmail"));
                    if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactName"))) newCustomerItem.TechnicalContactName = reader.GetString(reader.GetOrdinal("TechnicalContactName"));
                    if (!reader.IsDBNull(reader.GetOrdinal("TechnicalContactEmail"))) newCustomerItem.TechnicalContactEmail = reader.GetString(reader.GetOrdinal("TechnicalContactEmail"));
                    if (!reader.IsDBNull(reader.GetOrdinal("IndustryDescription"))) newCustomerItem.IndustryDescription = reader.GetString(reader.GetOrdinal("IndustryDescription"));

                    lstReturn.Add(newCustomerItem);

                }

            }

            return lstReturn;

        }

    }

}
