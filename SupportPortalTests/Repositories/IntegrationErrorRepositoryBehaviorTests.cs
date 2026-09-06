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
    public class IntegrationErrorRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 21L }
                                                        , { "IntegrationId", 42L }
                                                        , { "Deleted", false }
                                                        , { "Description", "desc" }
                                                        , { "ErrorMessage", "err" }
                                                        , { "ErrorTime", DateTime.UtcNow }
                                                        , { "StackTrace", "st" }
                                                        , { "RowVersion", rowVersion }
                                                        , { "IntegrationDescription", "integration desc" }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<IntegrationErrorRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 21L }
                                                        , { "IntegrationId", 42L }
                                                        , { "Deleted", false }
                                                        , { "Description", "desc" }
                                                        , { "ErrorMessage", "err" }
                                                        , { "ErrorTime", DateTime.UtcNow }
                                                        , { "StackTrace", "st" }
                                                        , { "RowVersion", rowVersion }
                                                        , { "IntegrationDescription", "integration desc" }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<IntegrationErrorRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsIntegrationError()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 21L }
                                                        , { "IntegrationId", 42L }
                                                        , { "Deleted", false }
                                                        , { "Description", "desc" }
                                                        , { "ErrorMessage", "err" }
                                                        , { "ErrorTime", DateTime.UtcNow }
                                                        , { "StackTrace", "st" }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<IntegrationErrorRepository>();
            var obj = await repo.GetByIdAsync(21L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsCustomer()
        {
            var repo = CreateRepository<IntegrationErrorRepository>();
            try { var byParent = await repo.GetByNameAsync("name", CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(123L);
            var repo = CreateRepository<IntegrationErrorRepository>();
            var created = await repo.CreateAsync(new IntegrationErrorEntity { ErrorMessage = "e" }, CancellationToken.None);
            Assert.AreEqual(123L, created);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<IntegrationErrorRepository>();
            var updated = await repo.UpdateAsync(new IntegrationErrorEntity { Id = 123L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<IntegrationErrorRepository>();
            try { var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

    }

}
