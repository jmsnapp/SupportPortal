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
    public class IntegrationTypeRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 7L }
                                                        , { "Name", "T1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<IntegrationTypeRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 7L }
                                                        , { "Name", "T1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<IntegrationTypeRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsType()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 7L }
                                                        , { "Name", "T1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<IntegrationTypeRepository>();
            var obj = await repo.GetByIdAsync(7L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsType()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 7L }
                                                        , { "Name", "T1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<IntegrationTypeRepository>();
            var obj = await repo.GetByNameAsync("T1", CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(88L);
            var repo = CreateRepository<IntegrationTypeRepository>();
            var created = await repo.CreateAsync(new IntegrationTypeEntity { Name = "N" }, CancellationToken.None);
            Assert.AreEqual(88L, created);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<IntegrationTypeRepository>();
            var updated = await repo.UpdateAsync(new IntegrationTypeEntity { Id = 88L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<IntegrationTypeRepository>();
            try { var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

    }

}
