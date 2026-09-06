using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using SupportPortalTests.Repositories.TestHelpers;

namespace SupportPortalTests.Repositories
{
    [TestClass]
    public class IntegrationRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var integrationRow = new Dictionary<string, object?>
            {
                { "Id", 10L }
                , { "Name", "Int1" }
                , { "Description", "desc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
                , { "CustomerId", 20L }
                , { "IntegrationTypeId", 2L }
                , { "CurrentStatusId", 3L }
                , { "LastSuccessfulSync", DateTime.Parse("2020-01-01") }
                , { "LastFailedSync", DateTime.Parse("2020-01-02") }
                , { "RetryCount", 3 }
                , { "CustomerDescription", "cdesc" }
                , { "IntegrationTypeDescription", "tDesc" }
                , { "CurrentStatusDescription", "sDesc" }
            };

            var readerForGetAll = new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { integrationRow } });

            SetReader(readerForGetAll);
            var repo = CreateRepository<IntegrationRepository>();
            var list = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(list);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var integrationRow = new Dictionary<string, object?>
            {
                { "Id", 10L }
                , { "Name", "Int1" }
                , { "Description", "desc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
                , { "CustomerId", 20L }
                , { "IntegrationTypeId", 2L }
                , { "CurrentStatusId", 3L }
                , { "LastSuccessfulSync", DateTime.Parse("2020-01-01") }
                , { "LastFailedSync", DateTime.Parse("2020-01-02") }
                , { "RetryCount", 3 }
                , { "CustomerDescription", "cdesc" }
                , { "IntegrationTypeDescription", "tDesc" }
                , { "CurrentStatusDescription", "sDesc" }
            };

            var readerForGetAll = new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>> { integrationRow } });

            SetReader(readerForGetAll);
            var repo = CreateRepository<IntegrationRepository>();
            var list = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(list);

        }

        [TestMethod]
        public async Task GetByIdAsync_MapsNestedObjects()
        {
            var rowVersion = new byte[8];
            var integrationRow = new Dictionary<string, object?>
            {
                { "Id", 10L }
                , { "Name", "Int1" }
                , { "Description", "desc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
                , { "CustomerId", 20L }
                , { "IntegrationTypeId", 2L }
                , { "CurrentStatusId", 3L }
                , { "LastSuccessfulSync", DateTime.Parse("2020-01-01") }
                , { "LastFailedSync", DateTime.Parse("2020-01-02") }
                , { "RetryCount", 3 }
            };

            var typeRow = new Dictionary<string, object?>
            {
                { "Id", 2L }
                , { "Name", "TypeX" }
                , { "Description", "tDesc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
            };

            var statusRow = new Dictionary<string, object?>
            {
                { "Id", 3L }
                , { "Name", "StatusY" }
                , { "Description", "sDesc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
            };

            var customerRow = new Dictionary<string, object?>
            {
                { "Id", 20L }
                , { "Name", "Cust" }
                , { "Description", "cdesc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
                , { "IndustryId", 30L }
                , { "PrimaryContactName", "PC" }
                , { "PrimaryContactEmail", "pc@c" }
                , { "TechnicalContactName", "TC" }
                , { "TechnicalContactEmail", "tc@c" }
            };

            var industryRow = new Dictionary<string, object?>
            {
                { "Id", 30L }
                , { "Name", "Ind" }
                , { "Description", "inddesc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
            };

            var readerForGetById = new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { integrationRow }
                                                                                                    , new List<Dictionary<string, object?>> { typeRow }
                                                                                                    , new List<Dictionary<string, object?>> { statusRow }
                                                                                                    , new List<Dictionary<string, object?>> { customerRow }
                                                                                                    , new List<Dictionary<string, object?>> { industryRow } });

            SetReader(readerForGetById);
            var repo = CreateRepository<IntegrationRepository>();
            var obj = await repo.GetByIdAsync(10L, CancellationToken.None);
            Assert.IsNotNull(obj);
            Assert.AreEqual(10L, obj.Id);
            Assert.IsNotNull(obj.Type);
            Assert.IsNotNull(obj.CurrentStatus);
            Assert.IsNotNull(obj.Customer);
            Assert.IsNotNull(obj.Customer.Industry);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsIntegration()
        {
            var rowVersion = new byte[8];
            var integrationRow = new Dictionary<string, object?>
            {
                { "Id", 10L }
                , { "Name", "Int1" }
                , { "Description", "desc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
                , { "CustomerId", 20L }
                , { "IntegrationTypeId", 2L }
                , { "CurrentStatusId", 3L }
                , { "LastSuccessfulSync", DateTime.Parse("2020-01-01") }
                , { "LastFailedSync", DateTime.Parse("2020-01-02") }
                , { "RetryCount", 3 }
            };

            var typeRow = new Dictionary<string, object?>
            {
                { "Id", 2L }
                , { "Name", "TypeX" }
                , { "Description", "tDesc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
            };

            var statusRow = new Dictionary<string, object?>
            {
                { "Id", 3L }
                , { "Name", "StatusY" }
                , { "Description", "sDesc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
            };

            var customerRow = new Dictionary<string, object?>
            {
                { "Id", 20L }
                , { "Name", "Cust" }
                , { "Description", "cdesc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
                , { "IndustryId", 30L }
                , { "PrimaryContactName", "PC" }
                , { "PrimaryContactEmail", "pc@c" }
                , { "TechnicalContactName", "TC" }
                , { "TechnicalContactEmail", "tc@c" }
            };

            var industryRow = new Dictionary<string, object?>
            {
                { "Id", 30L }
                , { "Name", "Ind" }
                , { "Description", "inddesc" }
                , { "Deleted", false }
                , { "RowVersion", rowVersion }
            };

            var readerForGetById = new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { integrationRow }
                                                                                                    , new List<Dictionary<string, object?>> { typeRow }
                                                                                                    , new List<Dictionary<string, object?>> { statusRow }
                                                                                                    , new List<Dictionary<string, object?>> { customerRow }
                                                                                                    , new List<Dictionary<string, object?>> { industryRow } });

            SetReader(readerForGetById);
            var repo = CreateRepository<IntegrationRepository>();
            var obj = await repo.GetByNameAsync("Int1", CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(77L);
            var repo = CreateRepository<IntegrationRepository>();
            var id = await repo.CreateAsync(new IntegrationEntity { Name = "X" }, CancellationToken.None);
            Assert.AreEqual(77L, id);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<IntegrationRepository>();
            var updated = await repo.UpdateAsync(new IntegrationEntity { Id = 77L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_ThrowsNotImplemented()
        {
            var repo = CreateRepository<IntegrationRepository>();
            try { await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.Fail("Expected NotImplementedException"); } catch (NotImplementedException) { }

        }

    }

}
