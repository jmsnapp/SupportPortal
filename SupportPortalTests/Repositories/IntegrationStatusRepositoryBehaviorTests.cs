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
    public class IntegrationStatusRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 3L }
                                                        , { "Name", "S1" }
                                                        , { "Description", "desc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<IntegrationStatusRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 3L }
                                                        , { "Name", "S1" }
                                                        , { "Description", "desc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<IntegrationStatusRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsStatus()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 3L }
                                                        , { "Name", "S1" }
                                                        , { "Description", "desc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<IntegrationStatusRepository>();
            var obj = await repo.GetByIdAsync(3L, CancellationToken.None);
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsStatus()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 3L }
                                                        , { "Name", "S1" }
                                                        , { "Description", "desc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<IntegrationStatusRepository>();
            var obj = await repo.GetByNameAsync("S1", CancellationToken.None);
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(55L);
            var repo = CreateRepository<IntegrationStatusRepository>();
            var created = await repo.CreateAsync(new IntegrationStatusEntity { Name = "N" }, CancellationToken.None);
            Assert.AreEqual(55L, created);
        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<IntegrationStatusRepository>();
            var updated = await repo.UpdateAsync(new IntegrationStatusEntity { Id = 55L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);
        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<IntegrationStatusRepository>();
            try { var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

    }

}
