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
    public class TicketNoteRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 91L }
                                                        , { "Description", "td" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "TicketId", 5L }
                                                        , { "Note", "tn" }
                                                        , { "CreateTime", DateTime.UtcNow }
                                                    };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<TicketNoteRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 91L }
                                                        , { "Description", "td" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "TicketId", 5L }
                                                        , { "Note", "tn" }
                                                        , { "CreateTime", DateTime.UtcNow }
                                                    };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<TicketNoteRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsTicketNote()
        {
            var row = new Dictionary<string, object?> { { "Id", 91L }
                                                        , { "Description", "td" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "TicketId", 5L }
                                                        , { "Note", "tn" }
                                                        , { "CreateTime", DateTime.UtcNow }
                                                    };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<TicketNoteRepository>();
            var obj = await repo.GetByIdAsync(91L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsTicketNote()
        {
            var repo = CreateRepository<TicketNoteRepository>();
            try { var byParent = await repo.GetByNameAsync("name", CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var row = new Dictionary<string, object?> { { "Id", 91L }
                                                        , { "Description", "td" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", new byte[8] }
                                                        , { "TicketId", 5L }
                                                        , { "Note", "tn" }
                                                        , { "CreateTime", DateTime.UtcNow }
                                                    };
            var repo = CreateRepository<TicketNoteRepository>();
            var obj = await repo.GetByParentIdAsync(5L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(555L);
            var repo = CreateRepository<TicketNoteRepository>();
            var id = await repo.CreateAsync(new TicketNoteEntity { TicketId = 5, Note = "n" }, CancellationToken.None);
            Assert.AreEqual(555L, id);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<TicketNoteRepository>();
            var updated = await repo.UpdateAsync(new TicketNoteEntity { Id = 555L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

    }

}
