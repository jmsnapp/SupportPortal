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
public class EscalationsControllerTests
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
        var entity = new Escalation { Id = 1L, Description = "Open" };

        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var controller = new EscalationsController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Escalation;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Description);
    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((Escalation?)null);

        var controller = new EscalationsController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        var entities = new List<Escalation>
        {
            new Escalation { Id = 1L, Description = "A" },
            new Escalation { Id = 2L, Description = "B" },
        };

        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((List<Escalation>)entities));

        EscalationsController controller = new EscalationsController(repoMock.Object);

        ActionResult<PagedResult<Escalation>> result = await controller.GetAll();

        PagedResult<Escalation> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        var entities = new List<Escalation>
        {
            new Escalation { Id = 3L, Description = "Active1" },
            new Escalation { Id = 4L, Description = "Active2" },
        };

        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((List<Escalation>)entities));

        EscalationsController controller = new EscalationsController(repoMock.Object);

        ActionResult<PagedResult<Escalation>> result = await controller.GetAllActive();

        PagedResult<Escalation> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        var controller = new EscalationsController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as Escalation);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        var updated = new Escalation { Id = 0L, Description = "X", Deleted = true };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((Escalation?)null);

        var controller = new EscalationsController(repoMock.Object);

        var updated = new Escalation { Id = 5L, Description = "Z" };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        var existing = new Escalation { Id = 6L, Description = "Before", Deleted = true };

        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<EscalationEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        var controller = new EscalationsController(repoMock.Object);

        var updated = new Escalation { Id = 6L, Description = "After", Deleted = true };
        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the ID, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<EscalationEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        var controller = new EscalationsController(repoMock.Object);

        var result = await controller.Create(null as Escalation);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        var toCreate = new Escalation { Id = -1L, Description = "New", Deleted = false };

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        var saved = new Escalation { Id = 7L, Description = "New", Deleted = false };

        var repoMock = new Mock<IGenericRepository<Escalation, Escalation, EscalationEntity>>();
        // Moq compares a literal argument with Equals, which EscalationEntity does not override,
        // so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<EscalationEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(7L);
        repoMock.Setup(r => r.GetByIdAsync(7L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        var controller = new EscalationsController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<Escalation, Escalation, EscalationEntity>.GetById), result!.ActionName);
        var model = result.Value as Escalation;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
