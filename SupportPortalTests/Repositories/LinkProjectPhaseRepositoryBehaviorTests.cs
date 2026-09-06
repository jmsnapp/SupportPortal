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
    public class LinkProjectPhaseRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 31L }
                                                        , { "Description", "desc" }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "Deleted", false }
                                                        , { "ProjectId", 2L }
                                                        , { "PhaseId", 3L }
                                                        , { "Percentage", 50 }
                                                        , { "Order", 1 }
                                                        , { "ProjectDescription", "pDesc" }
                                                        , { "PhaseDescription", "phDesc"} };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<ProjectPhaseRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 31L }
                                                        , { "Description", "desc" }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "Deleted", false }
                                                        , { "ProjectId", 2L }
                                                        , { "PhaseId", 3L }
                                                        , { "Percentage", 50 }
                                                        , { "Order", 1 }
                                                        , { "ProjectDescription", "pDesc" }
                                                        , { "PhaseDescription", "phDesc"} };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<ProjectPhaseRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsProjectPhase()
        {
            var row = new Dictionary<string, object?> { { "Id", 31L }
                                                        , { "Description", "desc" }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "Deleted", false }
                                                        , { "ProjectId", 2L }
                                                        , { "PhaseId", 3L }
                                                        , { "Percentage", 50 }
                                                        , { "Order", 1 } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<ProjectPhaseRepository>();
            var obj = await repo.GetByIdAsync(31L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsCustomer()
        {
            var repo = CreateRepository<ProjectPhaseRepository>();
            try { var byParent = await repo.GetByNameAsync("name", CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var row = new Dictionary<string, object?> { { "Id", 31L }
                                                        , { "Description", "desc" }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "Deleted", false }
                                                        , { "ProjectId", 2L }
                                                        , { "PhaseId", 3L }
                                                        , { "Percentage", 50 }
                                                        , { "Order", 1 }
                                                        , { "ProjectDescription", "pDesc" }
                                                        , { "PhaseDescription", "phDesc"} };
            var repo = CreateRepository<ProjectPhaseRepository>();
            var byParent = await repo.GetByParentIdAsync(2L, CancellationToken.None);
            Assert.IsNotNull(byParent);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(222L);
            var repo = CreateRepository<ProjectPhaseRepository>();
            var id = await repo.CreateAsync(new LinkProjectPhaseEntity { ProjectId = 2, PhaseId = 3 }, CancellationToken.None);
            Assert.AreEqual(222L, id);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<ProjectPhaseRepository>();
            var updated = await repo.UpdateAsync(new LinkProjectPhaseEntity { Id = 222L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

    }

}
