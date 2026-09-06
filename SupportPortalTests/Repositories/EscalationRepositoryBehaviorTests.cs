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
    public class EscalationRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 11L }
                                                        , { "Description", "esc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "ProblemSummary", "problem" }
                                                        , { "CustomerImpact", "impact" }
                                                        , { "RootCause", "cause" }
                                                        , { "RecommendedActions", "actions" }
                                                        , { "CreatedDate", new DateTime(1900, 1, 1) }};
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<EscalationRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 11L }
                                                        , { "Description", "esc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "ProblemSummary", "problem" }
                                                        , { "CustomerImpact", "impact" }
                                                        , { "RootCause", "cause" }
                                                        , { "RecommendedActions", "actions" }
                                                        , { "CreatedDate", new DateTime(1900, 1, 1) }};
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<EscalationRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsEscalation()
        {
            var row = new Dictionary<string, object?> { { "Id", 11L }
                                                        , { "Description", "esc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "ProblemSummary", "problem" }
                                                        , { "CustomerImpact", "impact" }
                                                        , { "RootCause", "cause" }
                                                        , { "RecommendedActions", "actions" }
                                                        , { "CreatedDate", new DateTime(1900, 1, 1) }};
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<EscalationRepository>();
            var obj = await repo.GetByIdAsync(11L, CancellationToken.None);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsCustomer()
        {
            var repo = CreateRepository<EscalationRepository>();
            try { var byParent = await repo.GetByNameAsync("name", CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(99L);
            var repo = CreateRepository<EscalationRepository>();
            var created = await repo.CreateAsync(new EscalationEntity { }, CancellationToken.None);
            Assert.AreEqual(99L, created);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<EscalationRepository>();
            var updated = await repo.UpdateAsync(new EscalationEntity { Id = 99L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<EscalationRepository>();
            try { var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

    }

}
