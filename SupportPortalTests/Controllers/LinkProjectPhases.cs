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
public class LinkProjectPhasesControllerTests
{
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
        LinkProjectPhaseEntity entity = new LinkProjectPhaseEntity { Id = 1L, Name = "Open", PhaseId = 1L };

        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as ProjectPhase;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((LinkProjectPhaseEntity?)null);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        LinkProjectPhaseEntity entity = new LinkProjectPhaseEntity { Id = 2L, Name = "Closed", PhaseId = 1L };

        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("Closed", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var result = await controller.GetByName("Closed") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as ProjectPhase;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("Closed", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        var entities = new List<LinkProjectPhaseEntity>
        {
            new LinkProjectPhaseEntity { Id = 1L, Name = "A", PhaseId = 1L },
            new LinkProjectPhaseEntity { Id = 2L, Name = "B", PhaseId = 1L },
        };

        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        repoMock
            .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<LinkProjectPhaseEntity>)entities, entities.Count));

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        ActionResult<PagedResult<ProjectPhase>> result = await controller.GetAll();

        PagedResult<ProjectPhase> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetPageAsync(0, 50, true, It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        var entities = new List<LinkProjectPhaseEntity>
        {
            new LinkProjectPhaseEntity { Id = 3L, Name = "Active1", PhaseId = 1L },
            new LinkProjectPhaseEntity { Id = 4L, Name = "Active2", PhaseId = 1L },
        };

        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        repoMock
            .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<LinkProjectPhaseEntity>)entities, entities.Count));

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        ActionResult<PagedResult<ProjectPhase>> result = await controller.GetAllActive();

        PagedResult<ProjectPhase> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetPageAsync(0, 50, false, It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as ProjectPhase);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        var entity= new LinkProjectPhaseEntity { Id = 2L, Name = "X" };
        ProjectPhase updated = new ProjectPhase();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((LinkProjectPhaseEntity?)null);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var entity = new LinkProjectPhaseEntity { Id = 5L, Name = "Z", PhaseId = 1L };
        ProjectPhase updated = new ProjectPhase();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        LinkProjectPhaseEntity existing = new LinkProjectPhaseEntity { Id = 6L, Name = "Before", PhaseId = 1L };

        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.Update(It.IsAny<LinkProjectPhaseEntity>())).Verifiable();
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var entity = new LinkProjectPhaseEntity { Id = 6L, Name = "After", PhaseId = 1L };
        ProjectPhase updated = new ProjectPhase();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the model, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.Update(It.IsAny<LinkProjectPhaseEntity>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var result = await controller.Create(null as ProjectPhase);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        LinkProjectPhaseEntity toCreate = new LinkProjectPhaseEntity { Id = 7L, Name = "New", PhaseId = 1L };

        Mock<IGenericRepository<LinkProjectPhaseEntity>> repoMock = new Mock<IGenericRepository<LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.AddAsync(toCreate, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repoMock.Setup(r => r.GetByIdAsync(-1L, It.IsAny<CancellationToken>())).ReturnsAsync(toCreate);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        ProjectPhase created = new ProjectPhase();
        DBMapper.MapPortalEntity2Object(toCreate, created);
        var result = await controller.Create(created) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<LinkProjectPhaseEntity, ProjectPhase>.GetById), result!.ActionName);
        var model = result.Value as ProjectPhase;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
