using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using SupportPortalTests.Repositories.TestHelpers;

namespace SupportPortalTests.Repositories
{
    [TestClass]
    public class IndustryRepositoryBehaviorTests : RepositoryTestFixture
    {
        [TestMethod]
        public async Task GetAllAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 2L }
                                                        , { "Name", "Ind" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            var readerForGetAll = new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } });
            SetReader(readerForGetAll);
            var repo = CreateRepository<IndustryRepository>();
            var all = await repo.GetAllAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetAllActiveAsync_ReturnsList()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 2L }
                                                        , { "Name", "Ind" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            var readerForGetAll = new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } });
            SetReader(readerForGetAll);
            var repo = CreateRepository<IndustryRepository>();
            var all = await repo.GetAllActiveAsync(CancellationToken.None);
            Assert.IsNotNull(all);

        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsIndustry()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 2L }
                                                        , { "Name", "Ind" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            var readerForGetById = new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } });
            SetReader(readerForGetById);
            var repo = CreateRepository<IndustryRepository>();
            var obj = await repo.GetByIdAsync(2L, CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task GetByNameAsync_ReturnsIndustry()
        {
            var rowVersion = new byte[8];
            var row = new Dictionary<string, object?> { { "Id", 2L }
                                                        , { "Name", "Ind" }
                                                        , { "Description", "d" }
                                                        , { "Deleted", false }
                                                        , { "RowVersion", rowVersion } };
            var readerForGetByName = new FakeDbDataReader(new List<List<Dictionary<string, object?>>>{ new List<Dictionary<string, object?>>{ row } });
            SetReader(readerForGetByName);
            var repo = CreateRepository<IndustryRepository>();
            var obj = await repo.GetByNameAsync("Ind", CancellationToken.None);
            Assert.IsNotNull(obj);

        }

        [TestMethod]
        public async Task CreateAsync_ReturnsNewId()
        {
            SetScalar(5L);
            var repo = CreateRepository<IndustryRepository>();
            var created = await repo.CreateAsync(new IndustryEntity { Name = "N" }, CancellationToken.None);
            Assert.AreEqual(5L, created);

        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsRowsAffected()
        {
            SetScalar(1L);
            var repo = CreateRepository<IndustryRepository>();
            var updated = await repo.UpdateAsync(new IndustryEntity { Id = 5L }, CancellationToken.None);
            Assert.AreEqual(1L, updated);

        }

        [TestMethod]
        public async Task GetByParentIdAsync_MayThrowOrReturn()
        {
            var repo = CreateRepository<IndustryRepository>();
            try
            {
                var byParent = await repo.GetByParentIdAsync(1L, CancellationToken.None);
                Assert.IsNotNull(byParent);

            }

            catch (NotImplementedException) { }

        }

    }

}
