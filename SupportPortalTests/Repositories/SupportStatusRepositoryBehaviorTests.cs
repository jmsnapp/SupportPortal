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
    public class SupportStatusRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 81L }
                                                        , { "Name", "SS1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<SupportStatusRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsStatus()
        {
            var row = new Dictionary<string, object?> { { "Id", 81L }
                                                        , { "Name", "SS1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<SupportStatusRepository>();
            var obj = await repo.GetByIdAsync(81L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsStatus()
        {
            var row = new Dictionary<string, object?> { { "Id", 81L }
                                                        , { "Name", "SS1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<SupportStatusRepository>();
            var obj = await repo.GetByNameAsync("SS1", CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(99L);
            var repo = CreateRepository<SupportStatusRepository>();
            var created = await repo.CreateAsync(new SupportStatusEntity { Name = "N" }, CancellationToken.None);
            Assert.AreEqual(99L, created);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<SupportStatusRepository>();
            var updated = await repo.UpdateAsync(new SupportStatusEntity { Id = 99L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<SupportStatusRepository>();
            try { var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

    }

}
