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
public class IntegrationStatusesControllerTests
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
        IntegrationStatus entity = new IntegrationStatus { Id = 1L, Name = "DEFAULT", Description = "Default", Deleted = false };

        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as IntegrationStatus;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((IntegrationStatus?)null);

        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        var result = await controller.GetById(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        IntegrationStatus entity = new IntegrationStatus { Id = 2L, Name = "DEFAULT", Description = "Default", Deleted = false };

        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("DEFAULT", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        var result = await controller.GetByName("DEFAULT") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as IntegrationStatus;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        List<IntegrationStatus> entities = new List<IntegrationStatus>
        {
            new IntegrationStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = false },
            new IntegrationStatus { Id = 1L, Name = "TEST", Description = "Test", Deleted = true }
        };

        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        ActionResult<PagedResult<IntegrationStatus>> result = await controller.GetAll();

        PagedResult<IntegrationStatus> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        List<IntegrationStatus> entities = new List<IntegrationStatus>
        {
            new IntegrationStatus { Id = 1L, Name = "OPERATIONAL", Description = "Operational", Deleted = false  },
            new IntegrationStatus { Id = 2L, Name = "OUT_OF_SERVICE", Description = "Out of Service", Deleted = false  }
        };

        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        ActionResult<PagedResult<IntegrationStatus>> result = await controller.GetAllActive();

        PagedResult<IntegrationStatus> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as IntegrationStatus);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        var updated  = new IntegrationStatus { Id = 2L, Name = "X" };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((IntegrationStatus?)null);

        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        var updated = new IntegrationStatus { Id = 5L, Name = "STAGING", Description = "Staging", Deleted = false };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        IntegrationStatus existing = new IntegrationStatus { Id = 6L, Name = "DEFAULT", Description = "Test", Deleted = true };

        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<IntegrationStatusEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        IntegrationStatus updated = new IntegrationStatus { Id = 6L, Name = "DEFAULT", Description = "Test", Deleted = true };
        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the model, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<IntegrationStatusEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        var result = await controller.Create(null as IntegrationStatus);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        IntegrationStatus toCreate = new IntegrationStatus { Id = 4L, Name = "DEFAULT", Description = "Test", Deleted = true };

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        IntegrationStatus saved = new IntegrationStatus { Id = 7L, Name = "DEFAULT", Description = "Test", Deleted = true };

        var repoMock = new Mock<IGenericRepository<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>>();
        // Moq compares a literal argument with Equals, which IntegrationStatusEntity does not
        // override, so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<IntegrationStatusEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(7L);
        repoMock.Setup(r => r.GetByIdAsync(7L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        IntegrationStatusesController controller = new IntegrationStatusesController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<IntegrationStatus, IntegrationStatus, IntegrationStatusEntity>.GetById), result!.ActionName);
        var model = result.Value as IntegrationStatus;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
