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

namespace SupportPortalTests.Controllers;

[TestClass]
public class SeveritiesControllerTests
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
        Severity entity = new Severity { Id = 1L, Name = "DEFAULT", Description = "Default", Deleted = true };

        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Severity;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((Severity?)null);

        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        Severity entity = new Severity { Id = 2L, Name = "DEFAULT", Description = "Default", Deleted = true };

        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("DEFAULT", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        var result = await controller.GetByName("DEFAULT") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Severity;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        Severity entity1 = new Severity { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Severity entity2 = new Severity { Id = 1L, Name = "LOW", Description = "Low", Deleted = false };

        List<Severity> entities = new List<Severity>();
        entities.Add(entity1);
        entities.Add(entity2);

        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        ActionResult<PagedResult<Severity>> result = await controller.GetAll();

        PagedResult<Severity> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        Severity entity1 = new Severity { Id = 1L, Name = "LOW", Description = "Low", Deleted = false };
        Severity entity2 = new Severity { Id = 2L, Name = "MEDIUM", Description = "Medium", Deleted = false };

        List<Severity> entities = new List<Severity>();
        entities.Add(entity1);
        entities.Add(entity2);

        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        ActionResult<PagedResult<Severity>> result = await controller.GetAllActive();

        PagedResult<Severity> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as Severity);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        Severity updated = new Severity { Id = 2L, Name = "MEDIUM", Description = "Medium", Deleted = false };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((Severity?)null);

        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        Severity updated = new Severity { Id = 5L, Name = "TEST", Description = "TEST", Deleted = false };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        Severity existing = new Severity { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };

        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(0L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<SeverityEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        Severity updated = new Severity { Id = 0L, Name = "TEST", Description = "Default", Deleted = true };
        var result = await controller.Update(0L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the ID, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<SeverityEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        var result = await controller.Create(null as Severity);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        Severity toCreate = new Severity { Id = 5L, Name = "TEST", Description = "TEST", Deleted = false };

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        Severity saved = new Severity { Id = 7L, Name = "TEST", Description = "TEST", Deleted = false };

        var repoMock = new Mock<IGenericRepository<Severity, Severity, SeverityEntity>>();
        // Moq compares a literal argument with Equals, which SeverityEntity does not override,
        // so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<SeverityEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(7L);
        repoMock.Setup(r => r.GetByIdAsync(7L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        SeveritiesController controller = new SeveritiesController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<Severity, Severity, SeverityEntity>.GetById), result!.ActionName);
        var model = result.Value as Severity;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
