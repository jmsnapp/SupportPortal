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
    public class SeverityRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 71L }
                                                        , { "Name", "Sev1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<SeverityRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None) as List<Severity>;
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 71L }
                                                        , { "Name", "Sev1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<SeverityRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None) as List<Severity>;
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsSeverity()
        {
            var row = new Dictionary<string, object?> { { "Id", 71L }
                                                        , { "Name", "Sev1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<SeverityRepository>();
            var obj = await repo.GetByIdAsync(71L, CancellationToken.None) as Severity;
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsSeverity()
        {
            var row = new Dictionary<string, object?> { { "Id", 71L }
                                                        , { "Name", "Sev1" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] } };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<SeverityRepository>();
            var obj = await repo.GetByNameAsync("Sev1", CancellationToken.None) as Severity;
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(77L);
            var repo = CreateRepository<SeverityRepository>();
            var created = await repo.CreateAsync(new SeverityEntity { Name = "N" }, CancellationToken.None);
            Assert.AreEqual(77L, created);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<SeverityRepository>();
            var updated = await repo.UpdateAsync(new SeverityEntity { Id = 77L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<SeverityRepository>();
            try { var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None) as List<Severity>; Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

    }

}
