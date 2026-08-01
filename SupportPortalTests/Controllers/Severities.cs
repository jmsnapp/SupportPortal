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
using SupportPortalDomain;
using SupportPortalDomain.Models;

namespace SupportPortalTests.Controllers;

[TestClass]
public class SeveritiesControllerTests
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

    private DBMapper _mapper = new DBMapper();

    [TestMethod]
    public async Task GetById_ReturnsOk_WhenEntityFound()
    {
        SeverityEntity entity = new SeverityEntity { Id = 1L, Name = "Open" };

        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Severity;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((SeverityEntity?)null);

        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        SeverityEntity entity = new SeverityEntity { Id = 2L, Name = "Closed" };
        IQueryable<SeverityEntity> list = new List<SeverityEntity> { entity }.AsQueryable();

        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        var result = await controller.GetByName("Closed") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Severity;
        Assert.IsNotNull(model);
        Assert.AreEqual(2, model.Id);
        Assert.AreEqual("Closed", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        List<SeverityEntity> entities = new List<SeverityEntity>
        {
            new SeverityEntity { Id = 1L, Name = "A" },
            new SeverityEntity { Id = 2L, Name = "B" },
        };

        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.IsNotNull(result);
        var models = result!.Value as IEnumerable<Severity>;
        Assert.IsNotNull(models);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), models!.Select(m => m.Id).ToList());

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        List<SeverityEntity> entities = new List<SeverityEntity>
        {
            new SeverityEntity { Id = 3L, Name = "Active1" },
            new SeverityEntity { Id = 4L, Name = "Active2" },
        };

        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        repoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        var result = await controller.GetAllActive() as OkObjectResult;

        Assert.IsNotNull(result);
        var models = result!.Value as IEnumerable<Severity>;
        Assert.IsNotNull(models);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), models!.Select(m => m.Id).ToList());

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        var badResult1 = await controller.Update(1L, null as SeverityEntity);
        Assert.IsInstanceOfType(badResult1, typeof(BadRequestResult));

        SeverityEntity updated = new SeverityEntity { Id = 2L, Name = "X" };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((SeverityEntity?)null);

        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        SeverityEntity updated = new SeverityEntity { Id = 5L, Name = "Z" };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNoContent_OnSuccess()
    {
        SeverityEntity existing = new SeverityEntity { Id = 6L, Name = "Before" };

        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.Update(It.IsAny<SeverityEntity>())).Verifiable();
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        SeverityEntity updated = new SeverityEntity { Id = 6L, Name = "After" };
        var result = await controller.Update(6L, updated);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        repoMock.Verify(r => r.Update(It.IsAny<SeverityEntity>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        var result = await controller.Create(null as SeverityEntity);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        SeverityEntity toCreate = new SeverityEntity { Id = 7L, Name = "New" };

        Mock<IGenericRepository<SeverityEntity>> repoMock = new Mock<IGenericRepository<SeverityEntity>>();
        repoMock.Setup(r => r.AddAsync(toCreate, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        SeveritiesController controller = new SeveritiesController(repoMock.Object, _mapper);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<SeverityEntity, Severity>.GetById), result!.ActionName);
        var model = result.Value as Severity;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
