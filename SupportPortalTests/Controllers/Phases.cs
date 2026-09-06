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
public class PhasesControllerTests
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
        Phase entity = new Phase { Id = 1L, Name = "DEFAULT", Description = "Default", Deleted = false };

        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        PhasesController controller = new PhasesController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Phase;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((Phase?)null);

        PhasesController controller = new PhasesController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        Phase entity = new Phase { Id = 2L, Name = "DEFAULT", Description = "Default", Deleted = true };

        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("DEFAULT", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        PhasesController controller = new PhasesController(repoMock.Object);

        var result = await controller.GetByName("DEFAULT") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Phase;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        Phase entity1 = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Phase entity2 = new Phase { Id = 1L, Name = "BUS_REQ_DISCOVERY", Description = "Business Requirements Discovery", Deleted = false };

        List<Phase> entities = new List<Phase>();
        entities.Add(entity1);
        entities.Add(entity2);

        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        PhasesController controller = new PhasesController(repoMock.Object);

        ActionResult<PagedResult<Phase>> result = await controller.GetAll();

        PagedResult<Phase> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        Phase entity1 = new Phase { Id = 1L, Name = "BUS_REQ_DISCOVERY", Description = "Business Requirements Discovery", Deleted = false };
        Phase entity2 = new Phase { Id = 2L, Name = "TECH_REQ_DISCOVERY", Description = "Technical Requirements Discovery", Deleted = false };

        List<Phase> entities = new List<Phase>();
        entities.Add(entity1);
        entities.Add(entity2);

        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        PhasesController controller = new PhasesController(repoMock.Object);

        ActionResult<PagedResult<Phase>> result = await controller.GetAllActive();

        PagedResult<Phase> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        PhasesController controller = new PhasesController(repoMock.Object);

        var badResult1 = await controller.Update(1, null as Phase);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        var updated = new Phase { Id = 7L, Name = "DEFAULT", Description = "Default", Deleted = true };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(7L, It.IsAny<CancellationToken>())).ReturnsAsync((Phase?)null);

        PhasesController controller = new PhasesController(repoMock.Object);

        var updated = new Phase { Id = 7L, Name = "DEFAULT", Description = "Default", Deleted = true };
        var result = await controller.Update(7L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        Phase existing = new Phase { Id = 6L, Name = "DEFAULT", Description = "Default", Deleted = true };

        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<PhaseEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        PhasesController controller = new PhasesController(repoMock.Object);

        var updated = new Phase { Id = 6L, Name = "DEFAULT", Description = "Default", Deleted = true };
        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the model, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<PhaseEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        PhasesController controller = new PhasesController(repoMock.Object);

        var result = await controller.Create(null as Phase);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        Phase toCreate = new Phase { Id = 7L, Name = "Test", Description = "Test", Deleted = false };

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        Phase saved = new Phase { Id = 7L, Name = "Test", Description = "Test", Deleted = false };

        var repoMock = new Mock<IGenericRepository<Phase, Phase, PhaseEntity>>();
        // Moq compares a literal argument with Equals, which PhaseEntity does not override,
        // so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<PhaseEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(7L);
        repoMock.Setup(r => r.GetByIdAsync(7L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        PhasesController controller = new PhasesController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<Phase, Phase, PhaseEntity>.GetById), result!.ActionName);
        var model = result.Value as Phase;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
