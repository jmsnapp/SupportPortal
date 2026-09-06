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
        ProjectPhase entity = new ProjectPhase { Id = 1L, ProjectId = 0, Order = 0, Percentage = 0, Description = "Default", Deleted = true };
        Phase phase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = false };
        entity.Phase = phase;

        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as ProjectPhase;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Default", model.Description);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((ProjectPhase?)null);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        ProjectPhase entity1 = new ProjectPhase { Id = 0L, ProjectId = 0, Order = 0, Percentage = 0, Description = "Default", Deleted = true };
        ProjectPhase entity2 = new ProjectPhase { Id = 1L, ProjectId = 1, Order = 0, Percentage = 0, Description = "1_1", Deleted = false };
        Phase phase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = false };
        entity1.Phase = phase;
        entity2.Phase = phase;

        List<ProjectPhase> entities = new List<ProjectPhase>() { entity1, entity2 };

        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        ActionResult<PagedResult<ProjectPhase>> result = await controller.GetAll();

        PagedResult<ProjectPhase> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        ProjectPhase entity1 = new ProjectPhase { Id = 1L, ProjectId = 1, Order = 0, Percentage = 0, Description = "1_1", Deleted = false };
        ProjectPhase entity2 = new ProjectPhase { Id = 2L, ProjectId = 1, Order = 0, Percentage = 0, Description = "1_2", Deleted = false };
        Phase phase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = false };
        entity1.Phase = phase;
        entity2.Phase = phase;

        List<ProjectPhase> entities = new List<ProjectPhase>() { entity1, entity2 };

        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        ActionResult<PagedResult<ProjectPhase>> result = await controller.GetAllActive();

        PagedResult<ProjectPhase> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as ProjectPhase);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        var updated = new ProjectPhase { Id = 0L, ProjectId = 0, Order = 0, Percentage = 0, Description = "Default", Deleted = true };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((ProjectPhase?)null);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var updated = new ProjectPhase { Id = 5L, ProjectId = 0, Order = 0, Percentage = 0, Description = "Default", Deleted = true };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        ProjectPhase existing = new ProjectPhase { Id = 6L, ProjectId = 0, Order = 0, Percentage = 0, Description = "Default", Deleted = true };

        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<LinkProjectPhaseEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var updated = new ProjectPhase { Id = 6L, ProjectId = 0, Order = 0, Percentage = 0, Description = "Test", Deleted = true };
        LinkProjectPhaseEntity updatedEntity = new LinkProjectPhaseEntity();
        DBMapper.MapProjectPhase2LinkProjectPhaseEntity(updated, ref updatedEntity);
        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the ID, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<LinkProjectPhaseEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var result = await controller.Create(null as ProjectPhase);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        ProjectPhase toCreate = new ProjectPhase { Id = 4L, ProjectId = 0, Order = 0, Percentage = 0, Description = "Default", Deleted = true };

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        ProjectPhase saved = new ProjectPhase { Id = 7L, ProjectId = 0, Order = 0, Percentage = 0, Description = "Default", Deleted = true };

        var repoMock = new Mock<IGenericRepository<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>>();
        // Moq compares a literal argument with Equals, which LinkProjectPhaseEntity does not
        // override, so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<LinkProjectPhaseEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(7L);
        repoMock.Setup(r => r.GetByIdAsync(7L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        LinkProjectPhasesController controller = new LinkProjectPhasesController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<ProjectPhase, ProjectPhase, LinkProjectPhaseEntity>.GetById), result!.ActionName);
        var model = result.Value as ProjectPhase;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
