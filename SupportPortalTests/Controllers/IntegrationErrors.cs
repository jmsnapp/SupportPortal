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
        IntegrationErrorEntity entity = new IntegrationErrorEntity { Id = 1L, Name = "Open" };

        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as IntegrationError;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((IntegrationErrorEntity?)null);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        IntegrationErrorEntity entity = new IntegrationErrorEntity { Id = 2L, Name = "Closed" };

        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("Closed", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var result = await controller.GetByName("Closed") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as IntegrationError;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("Closed", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        List<IntegrationErrorEntity> entities = new List<IntegrationErrorEntity>
        {
            new IntegrationErrorEntity { Id = 1L, Name = "A" },
            new IntegrationErrorEntity { Id = 2L, Name = "B" },
        };

        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        repoMock
            .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<IntegrationErrorEntity>)entities, entities.Count));

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        ActionResult<PagedResult<IntegrationError>> result = await controller.GetAll();

        PagedResult<IntegrationError> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetPageAsync(0, 50, true, It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        List<IntegrationErrorEntity> entities = new List<IntegrationErrorEntity>
        {
            new IntegrationErrorEntity { Id = 3L, Name = "Active1" },
            new IntegrationErrorEntity { Id = 4L, Name = "Active2" },
        };

        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        repoMock
            .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<IntegrationErrorEntity>)entities, entities.Count));

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        ActionResult<PagedResult<IntegrationError>> result = await controller.GetAllActive();

        PagedResult<IntegrationError> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetPageAsync(0, 50, false, It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var badResult1 = await controller.Update(1, null as IntegrationError);
        Assert.IsInstanceOfType(badResult1, typeof(BadRequestResult));

        var entity = new IntegrationErrorEntity { Id = 2L, Name = "X" };
        IntegrationError updated = new IntegrationError();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((IntegrationErrorEntity?)null);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var entity = new IntegrationErrorEntity { Id = 5L, Name = "Z" };
        IntegrationError updated = new IntegrationError();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNoContent_OnSuccess()
    {
        IntegrationErrorEntity existing = new IntegrationErrorEntity { Id = 6L, Name = "Before" };

        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.Update(It.IsAny<IntegrationErrorEntity>())).Verifiable();
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        IntegrationErrorEntity entity = new IntegrationErrorEntity { Id = 6L, Name = "After" };
        IntegrationError updated = new IntegrationError();
        DBMapper.MapPortalEntity2Object(entity, updated);
        var result = await controller.Update(6L, updated);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        repoMock.Verify(r => r.Update(It.IsAny<IntegrationErrorEntity>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        var result = await controller.Create(null as IntegrationError);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        IntegrationErrorEntity toCreate = new IntegrationErrorEntity { Id = 7L, Name = "New" };

        Mock<IGenericRepository<IntegrationErrorEntity>> repoMock = new Mock<IGenericRepository<IntegrationErrorEntity>>();
        repoMock.Setup(r => r.AddAsync(toCreate, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repoMock.Setup(r => r.GetByIdAsync(-1L, It.IsAny<CancellationToken>())).ReturnsAsync(toCreate);

        IntegrationErrorsController controller = new IntegrationErrorsController(repoMock.Object);

        IntegrationError created = new IntegrationError();
        DBMapper.MapPortalEntity2Object(toCreate, created);
        var result = await controller.Create(created) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<IntegrationErrorEntity, IntegrationError>.GetById), result!.ActionName);
        var model = result.Value as IntegrationError;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
