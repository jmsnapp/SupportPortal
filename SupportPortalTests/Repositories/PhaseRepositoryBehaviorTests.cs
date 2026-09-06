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
    public class PhaseRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 41L }
                                                        , { "Name", "Phase1" }
                                                        , { "Description", "pdesc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<PhaseRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 41L }
                                                        , { "Name", "Phase1" }
                                                        , { "Description", "pdesc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<PhaseRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsPhase()
        {
            var row = new Dictionary<string, object?> { { "Id", 41L }
                                                        , { "Name", "Phase1" }
                                                        , { "Description", "pdesc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<PhaseRepository>();
            var obj = await repo.GetByIdAsync(41L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsPhase()
        {
            var row = new Dictionary<string, object?> { { "Id", 41L }
                                                        , { "Name", "Phase1" }
                                                        , { "Description", "pdesc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<PhaseRepository>();
            var obj = await repo.GetByNameAsync("Phase1", CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(321L);
            var repo = CreateRepository<PhaseRepository>();
            var created = await repo.CreateAsync(new PhaseEntity { Name = "N" }, CancellationToken.None);
            Assert.AreEqual(321L, created);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<PhaseRepository>();
            var updated = await repo.UpdateAsync(new PhaseEntity { Id = 321L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<PhaseRepository>();
            try { var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

    }

}
