using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using SupportPortalAPI.Controllers;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using SupportPortalInfrastructure;
using SupportPortalDomain.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace SupportPortalTests.Controllers;

[TestClass]
public class IndustriesControllerTests
{
    private static readonly Microsoft.Extensions.Options.IOptions<SupportPortalInfrastructure.Configuration.PaginationOptions> _options = Microsoft.Extensions.Options.Options.Create(new SupportPortalInfrastructure.Configuration.PaginationOptions());
    // Helper classes to allow EF Core async LINQ extensions to work against in-memory IQueryable
    private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        public TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }
        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
        public object? Execute(Expression expression) => _inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new TestAsyncEnumerable<TResult>(expression);
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Execute<TResult>(expression);
    }

    private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellation = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
        public ValueTask DisposeAsync() { _inner.Dispose(); return default; }
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        public T Current => _inner.Current;
    }

    [TestMethod]
    public async Task GetById_ReturnsOk_WhenEntityFound()
    {
        Industry entity = new Industry { Id = 1L, Name = "DEFAULT", Description = "Default", Deleted = false };

        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IndustriesController controller = new IndustriesController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Industry;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((Industry?)null);

        IndustriesController controller = new IndustriesController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        Industry entity = new Industry { Id = 2L, Name = "DEFAULT", Description = "Default", Deleted = true };

        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("DEFAULT", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IndustriesController controller = new IndustriesController(repoMock.Object);

        var result = await controller.GetByName("DEFAULT") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Industry;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        List<Industry> entities = new List<Industry>
        {
            new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true },
            new Industry { Id = 1L, Name = "AVIATION", Description = "Aviation", Deleted = false  }
        };

        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((List<Industry>)entities));

        IndustriesController controller = new IndustriesController(repoMock.Object);

        ActionResult<PagedResult<Industry>> result = await controller.GetAll();

        PagedResult<Industry> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        List<Industry> entities = new List<Industry>
        {
            new Industry { Id = 1L, Name = "AVIATION", Description = "Aviation", Deleted = false  },
            new Industry { Id = 2L, Name = "CONGLOMERATE", Description = "Conglomerate", Deleted = false  }
        };

        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((List<Industry>)entities));

        IndustriesController controller = new IndustriesController(repoMock.Object);

        ActionResult<PagedResult<Industry>> result = await controller.GetAllActive();

        PagedResult<Industry> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        IndustriesController controller = new IndustriesController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as Industry);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        var updated = new Industry { Id = 2L, Name = "X" };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((Industry?)null);

        IndustriesController controller = new IndustriesController(repoMock.Object);

        var updated = new Industry { Id = 5L, Name = "MANUFACTURING", Description = "Manufacturing", Deleted = false };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        Industry existing = new Industry { Id = 6L, Name = "MEDIA", Description = "Media", Deleted = false };

        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<IndustryEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        IndustriesController controller = new IndustriesController(repoMock.Object);

        var updated = new Industry { Id = 6L, Name = "MEDIA", Description = "Media", Deleted = false };
        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the Id, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<IndustryEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        IndustriesController controller = new IndustriesController(repoMock.Object);

        var result = await controller.Create(null as Industry);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        Industry toCreate = new Industry { Id = 7L, Name = "TECHNOLOGY", Description = "Technology", Deleted = false };

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        Industry saved = new Industry { Id = 7L, Name = "TECHNOLOGY", Description = "Technology", Deleted = false };

        var repoMock = new Mock<IGenericRepository<Industry, Industry, IndustryEntity>>();
        // Moq compares a literal argument with Equals, which IndustryEntity does not override,
        // so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<IndustryEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(7L);
        repoMock.Setup(r => r.GetByIdAsync(7L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        IndustriesController controller = new IndustriesController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<Industry, Industry, IndustryEntity>.GetById), result!.ActionName);
        var model = result.Value as Industry;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
