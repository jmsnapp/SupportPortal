using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using SupportPortalTests.Repositories.TestHelpers;

namespace SupportPortalTests.Repositories
{
    [TestClass]
    public class CustomerRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var listRow = new Dictionary<string, object?>
            {
                { "Id", 1L }
                , { "Name", "A" }
                , { "Description", "d1" }
                , { "Deleted", false }
                , { "RowVersion", new byte[8] }
                , { "IndustryId", 2L }
                , { "PrimaryContactName", "P1" }
                , { "PrimaryContactEmail", "p1" }
                , { "TechnicalContactName", "T1" }
                , { "TechnicalContactEmail", "t1" }
                , { "IndustryDescription", "ind1" }
            };

            var readerForGetAll = new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { listRow } });

            SetReader(readerForGetAll);
            var repo = CreateRepository<CustomerRepository>();
            var list = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("A", list[0].Name);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var listRow = new Dictionary<string, object?>
            {
                { "Id", 1L }
                , { "Name", "A" }
                , { "Description", "d1" }
                , { "Deleted", false }
                , { "RowVersion", new byte[8] }
                , { "IndustryId", 2L }
                , { "PrimaryContactName", "P1" }
                , { "PrimaryContactEmail", "p1" }
                , { "TechnicalContactName", "T1" }
                , { "TechnicalContactEmail", "t1" }
                , { "IndustryDescription", "ind1" }
            };

            var readerForGetAll = new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { listRow } });

            SetReader(readerForGetAll);
            var repo = CreateRepository<CustomerRepository>();
            var list = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);

        }

        [TestMethod]
        public async Task GetByIdAsync_MapsCustomerAndIndustry()
        {
            var rowVersion = new byte[8];
            var customerRow = new Dictionary<string, object?> { { "Id", 123L }
                                                                , { "Name", "Acme" }
                                                                , { "Description", "d1" }
                                                                , { "Deleted", false }
                                                                , { "RowVersion", rowVersion }
                                                                , { "IndustryId", 5L }
                                                                , { "PrimaryContactName", "John" }
                                                                , { "PrimaryContactEmail", "john@acme" }
                                                                , { "TechnicalContactName", "T1" }
                                                                , { "TechnicalContactEmail", "t1" } };
            var industryRow = new Dictionary<string, object?> { { "Id", 5L }
                                                                , { "Name", "IndustryName" }
                                                                , { "Description", "d1" }
                                                                , { "RowVersion", rowVersion }
                                                                , { "Deleted", false } };
            var readerForGetById = new FakeDbDataReader(new List<List<Dictionary<string, object?>>> {
                new List<Dictionary<string, object?>> { customerRow },
                new List<Dictionary<string, object?>> { industryRow }
            });

            SetReader(readerForGetById);
            var repo = CreateRepository<CustomerRepository>();
            var cust = await repo.GetByIdAsync(123L, CancellationToken.None);
            Assert.IsNotNull(cust);
            Assert.AreEqual(123L, cust.Id);
            Assert.AreEqual("Acme", cust.Name);
            Assert.IsNotNull(cust.Industry);
            Assert.AreEqual(5L, cust.Industry.Id);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsCustomer()
        {
            var rowVersion = new byte[8];
            var customerRow = new Dictionary<string, object?> { { "Id", 123L }
                                                                , { "Name", "Acme" }
                                                                , { "Description", "d1" }
                                                                , { "Deleted", false }
                                                                , { "RowVersion", rowVersion }
                                                                , { "IndustryId", 5L }
                                                                , { "PrimaryContactName", "John" }
                                                                , { "PrimaryContactEmail", "john@acme" }
                                                                , { "TechnicalContactName", "T1" }
                                                                , { "TechnicalContactEmail", "t1" } };
            var industryRow = new Dictionary<string, object?> { { "Id", 5L }
                                                                , { "Name", "IndustryName" }
                                                                , { "Description", "d1" }
                                                                , { "RowVersion", rowVersion }
                                                                , { "Deleted", false } };
            var readerForGetByName = new FakeDbDataReader(new List<List<Dictionary<string, object?>>> {
                new List<Dictionary<string, object?>> { customerRow },
                new List<Dictionary<string, object?>> { industryRow }
            });

            SetReader(readerForGetByName);
            var repo = CreateRepository<CustomerRepository>();
            var cust = await repo.GetByNameAsync("Acme", CancellationToken.None) as Customer;
            Assert.IsNotNull(cust);
            Assert.AreEqual("Acme", cust.Name);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(456L);
            var repo = CreateRepository<CustomerRepository>();
            var newId = await repo.CreateAsync(new CustomerEntity { Name = "New" }, CancellationToken.None);
            Assert.AreEqual(456L, newId);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<CustomerRepository>();
            var updated = await repo.UpdateAsync(new CustomerEntity { Id = 456L, Name = "Updated" }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_ThrowsNotImplemented()
        {
            var repo = CreateRepository<CustomerRepository>();
            try { await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.Fail("Expected NotImplementedException"); } catch (NotImplementedException) { }

        }

    }

}
