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
    public class TicketRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 101L }
                , { "Description", "desc" }
                , { "Deleted", false }
                , { "RowVersion", new byte[8] }
                , { "CustomerId", 1L }
                , { "IntegrationId", 2L }
                , { "SeverityId", 3L }
                , { "StatusId", 4L }
                , { "EscalationId", 5L }
                , { "Reproduce", "rep" }
                , { "ReportedBy", "rb" }
                , { "AssignedTo", "at" }
                , { "Resolution", "resolve" }
                , { "ResolutionDate", DateTime.UtcNow.AddDays(1) }
                , { "CreatedDate", DateTime.UtcNow }
                , { "CustomerDescription", "cd" }
                , { "IntegrationDescription", "id" }
                , { "SeverityDescription", "sd" }
                , { "StatusDescription", "std" }
                , { "EscalationDescription", "ed" }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<TicketRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var row = new Dictionary<string, object?> { { "Id", 101L }
                , { "Description", "desc" }
                , { "Deleted", false }
                , { "RowVersion", new byte[8] }
                , { "CustomerId", 1L }
                , { "IntegrationId", 2L }
                , { "SeverityId", 3L }
                , { "StatusId", 4L }
                , { "EscalationId", 5L }
                , { "Reproduce", "rep" }
                , { "ReportedBy", "rb" }
                , { "AssignedTo", "at" }
                , { "Resolution", "resolve" }
                , { "ResolutionDate", DateTime.UtcNow.AddDays(1) }
                , { "CreatedDate", DateTime.UtcNow }
                , { "CustomerDescription", "cd" }
                , { "IntegrationDescription", "id" }
                , { "SeverityDescription", "sd" }
                , { "StatusDescription", "std" }
                , { "EscalationDescription", "ed" }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>> { new List<Dictionary<string, object?>> { row } }));
            var repo = CreateRepository<TicketRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsTicket()
        {
            var row = new Dictionary<string, object?> { { "Id", 101L }
                , { "Description", "desc" }
                , { "Deleted", false }
                , { "RowVersion", new byte[8] }
                , { "CustomerId", 1L }
                , { "IntegrationId", 2L }
                , { "SeverityId", 3L }
                , { "StatusId", 4L }
                , { "EscalationId", 5L }
                , { "Reproduce", "rep" }
                , { "ReportedBy", "rb" }
                , { "AssignedTo", "at" }
                , { "Resolution", "resolve" }
                , { "ResolutionDate", DateTime.UtcNow.AddDays(1) }
                , { "CreatedDate", DateTime.UtcNow }
            };
            SetReader(new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } }));
            var repo = CreateRepository<TicketRepository>();
            var obj = await repo.GetByIdAsync(101L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<TicketRepository>();
            try { var byParent = await repo.GetByNameAsync("name", CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<TicketRepository>();
            try { var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None); Assert.IsNotNull(byParent); }
            catch (NotImplementedException) { }

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(666L);
            var repo = CreateRepository<TicketRepository>();
            var id = await repo.CreateAsync(new TicketEntity { ReportedBy = "rb" }, CancellationToken.None);
            Assert.AreEqual(666L, id);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<TicketRepository>();
            var updated = await repo.UpdateAsync(new TicketEntity { Id = 666L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

    }

}
