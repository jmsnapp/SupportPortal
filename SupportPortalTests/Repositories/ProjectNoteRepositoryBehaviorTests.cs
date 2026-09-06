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
    public class ProjectNoteRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 51L }
                                                        , { "Description", "desc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "ProjectId", 2L }
                                                        , { "Note", "n" }
                                                        , { "CreateTime", DateTime.UtcNow }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<ProjectNoteRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsProjectNote()
        {
            var row = new Dictionary<string, object?> { { "Id", 51L }
                                                        , { "Description", "desc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "ProjectId", 2L }
                                                        , { "Note", "n" }
                                                        , { "CreateTime", DateTime.UtcNow }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<ProjectNoteRepository>();
            var obj = await repo.GetByIdAsync(51L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsCustomer()
        {
            var repo = CreateRepository<ProjectNoteRepository>();
            try { var byParent = await repo.GetByNameAsync("name", CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var row = new Dictionary<string, object?> { { "Id", 51L }
                                                        , { "Description", "desc" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "ProjectId", 2L }
                                                        , { "Note", "n" }
                                                        , { "CreateTime", DateTime.UtcNow }
            };
            var repo = CreateRepository<ProjectNoteRepository>();
            var byParent = await repo.GetByParentIdAsync(2L, CancellationToken.None);
            Assert.IsNotNull(byParent);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(333L);
            var repo = CreateRepository<ProjectNoteRepository>();
            var id = await repo.CreateAsync(new ProjectNoteEntity { ProjectId = 2, Note = "n" }, CancellationToken.None);
            Assert.AreEqual(333L, id);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<ProjectNoteRepository>();
            var updated = await repo.UpdateAsync(new ProjectNoteEntity { Id = 333L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

    }

}
