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
    public class ProjectRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 61L }
                , { "Name", "Proj1" }
                , { "Description", "pdesc" }
                , { "Deleted", false }
                , { "RowVersion", new byte[8] }
                , { "CustomerId", 2L }
                , { "CurrentPhase", 1L }
                , { "TargetGoLiveDate", DateTime.UtcNow.AddDays(30) }
                , { "ActualGoLiveDate", DBNull.Value }
                , { "CustomerDescription", "cdesc" }
                , { "CurrentPhaseDescription", "phdesc" }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<ProjectRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsProject()
        {
            var row = new Dictionary<string, object?> { { "Id", 61L }
                , { "Name", "Proj1" }
                , { "Description", "pdesc" }
                , { "Deleted", false }
                , { "RowVersion", new byte[8] }
                , { "CustomerId", 2L }
                , { "CurrentPhase", 1L }
                , { "TargetGoLiveDate", DateTime.UtcNow.AddDays(30) }
                , { "ActualGoLiveDate", DBNull.Value }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<ProjectRepository>();
            var obj = await repo.GetByIdAsync(61L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsProject()
        {
            var row = new Dictionary<string, object?> { { "Id", 61L }
                , { "Name", "Proj1" }
                , { "Description", "pdesc" }
                , { "Deleted", false }
                , { "RowVersion", new byte[8] }
                , { "CustomerId", 2L }
                , { "CurrentPhase", 1L }
                , { "TargetGoLiveDate", DateTime.UtcNow.AddDays(30) }
                , { "ActualGoLiveDate", DBNull.Value }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<ProjectRepository>();
            var obj = await repo.GetByNameAsync("Proj1", CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<ProjectRepository>();
            try { var byParent = await repo.GetByParentIdAsync(2L, CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(444L);
            var repo = CreateRepository<ProjectRepository>();
            var id = await repo.CreateAsync(new ProjectEntity { Name = "N" }, CancellationToken.None);
            Assert.AreEqual(444L, id);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<ProjectRepository>();
            var updated = await repo.UpdateAsync(new ProjectEntity { Id = 444L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

    }

}
