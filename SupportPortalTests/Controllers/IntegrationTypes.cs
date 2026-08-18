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
public class IntegrationTypesControllerTests
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
        IntegrationTypeEntity entity = new IntegrationTypeEntity { Id = 1L, Name = "Open" };

        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        var result = await controller.GetById(1) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as IntegrationType;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((IntegrationTypeEntity?)null);

        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        IntegrationTypeEntity entity = new IntegrationTypeEntity { Id = 2L, Name = "Closed" };

        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("Closed", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        var result = await controller.GetByName("Closed") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as IntegrationType;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("Closed", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        List<IntegrationTypeEntity> entities = new List<IntegrationTypeEntity>
        {
            new IntegrationTypeEntity { Id = 1L, Name = "A" },
            new IntegrationTypeEntity { Id = 2L, Name = "B" },
        };

        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        repoMock
            .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<IntegrationTypeEntity>)entities, entities.Count));

        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        ActionResult<PagedResult<IntegrationType>> result = await controller.GetAll();

        PagedResult<IntegrationType> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetPageAsync(0, 50, true, It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        List<IntegrationTypeEntity> entities = new List<IntegrationTypeEntity>
        {
            new IntegrationTypeEntity { Id = 3L, Name = "Active1" },
            new IntegrationTypeEntity { Id = 4L, Name = "Active2" },
        };

        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        repoMock
            .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<IntegrationTypeEntity>)entities, entities.Count));

        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        ActionResult<PagedResult<IntegrationType>> result = await controller.GetAllActive();

        PagedResult<IntegrationType> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetPageAsync(0, 50, false, It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        var badResult1 = await controller.Update(1, null as IntegrationType);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        IntegrationTypeEntity entity = new IntegrationTypeEntity { Id = 2L, Name = "X" };
        IntegrationType updated = new IntegrationType();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((IntegrationTypeEntity?)null);

        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        IntegrationTypeEntity entity = new IntegrationTypeEntity { Id = 5L, Name = "Z" };
        IntegrationType updated = new IntegrationType();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        IntegrationTypeEntity existing = new IntegrationTypeEntity { Id = 6L, Name = "Before" };

        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.Update(It.IsAny<IntegrationTypeEntity>())).Verifiable();
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        IntegrationTypeEntity entity = new IntegrationTypeEntity { Id = 6L, Name = "After" };
        IntegrationType updated = new IntegrationType();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the model, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.Update(It.IsAny<IntegrationTypeEntity>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        var result = await controller.Create(null as IntegrationType);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        IntegrationTypeEntity toCreate = new IntegrationTypeEntity { Id = 7L, Name = "New" };

        Mock<IGenericRepository<IntegrationTypeEntity>> repoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        repoMock.Setup(r => r.AddAsync(toCreate, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repoMock.Setup(r => r.GetByIdAsync(-1L, It.IsAny<CancellationToken>())).ReturnsAsync(toCreate);

        IntegrationTypesController controller = new IntegrationTypesController(repoMock.Object);

        IntegrationType created = new IntegrationType();
        DBMapper.MapPortalEntity2Object(toCreate, created);

        var result = await controller.Create(created) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<IntegrationTypeEntity, IntegrationType>.GetById), result!.ActionName);
        var model = result.Value as IntegrationType;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
