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
public class IntegrationErrorsControllerTests
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
        IntegrationError entity = new IntegrationError { Id = 1L, Description = "Open", Deleted = false };

        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as IntegrationError;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Description);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((IntegrationError?)null);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        List<IntegrationError> entities = new List<IntegrationError>
        {
            new IntegrationError { Id = 1L, Description = "A", Deleted = false },
            new IntegrationError { Id = 2L, Description = "B", Deleted = false },
        };

        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((List<IntegrationError>)entities));

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        ActionResult<PagedResult<IntegrationError>> result = await controller.GetAll();

        PagedResult<IntegrationError> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        List<IntegrationError> entities = new List<IntegrationError>
        {
            new IntegrationError { Id = 3L, Description = "Active1", Deleted = false },
            new IntegrationError { Id = 4L, Description = "Active2", Deleted = false },
        };

        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((List<IntegrationError>)entities));

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        ActionResult<PagedResult<IntegrationError>> result = await controller.GetAllActive();

        PagedResult<IntegrationError> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var badResult1 = await controller.Update(1, null as IntegrationError);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        var updated = new IntegrationError { Id = 2L, Description = "B", Deleted = false };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((IntegrationError?)null);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var updated = new IntegrationError { Id = 5L, Description = "Z" };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        IntegrationError existing = new IntegrationError { Id = 6L, Description = "Before" };

        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<IntegrationErrorEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        IntegrationError updated = new IntegrationError { Id = 6L, Description = "After" };
        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the ID, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<IntegrationErrorEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var result = await controller.Create(null as IntegrationError);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        IntegrationError toCreate = new IntegrationError { Id = 7L, Description = "New" };

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        IntegrationError saved = new IntegrationError { Id = 7L, Description = "New" };

        var repoMock = new Mock<IGenericRepository<IntegrationError, IntegrationError, IntegrationErrorEntity>>();
        // Moq compares a literal argument with Equals, which IntegrationErrorEntity does not
        // override, so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<IntegrationErrorEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(7L);
        repoMock.Setup(r => r.GetByIdAsync(7L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<IntegrationError, IntegrationErrorListItem, IntegrationErrorEntity>.GetById), result!.ActionName);
        var model = result.Value as IntegrationError;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
