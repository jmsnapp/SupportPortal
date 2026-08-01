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
public class ProjectNotesControllerTests
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
        ProjectNoteEntity entity = new ProjectNoteEntity { Id = 1L, Name = "Open" };

        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as ProjectNote;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((ProjectNoteEntity?)null);

        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        ProjectNoteEntity entity = new ProjectNoteEntity { Id = 2L, Name = "Closed" };
        var list = new List<ProjectNoteEntity> { entity }.AsQueryable();

        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        var result = await controller.GetByName("Closed") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as ProjectNote;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("Closed", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        List<ProjectNoteEntity> entities = new List<ProjectNoteEntity>
        {
            new ProjectNoteEntity { Id = 1L, Name = "A" },
            new ProjectNoteEntity { Id = 2L, Name = "B" },
        };

        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.IsNotNull(result);
        var models = result!.Value as IEnumerable<ProjectNote>;
        Assert.IsNotNull(models);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), models!.Select(m => m.Id).ToList());

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        List<ProjectNoteEntity> entities = new List<ProjectNoteEntity>
        {
            new ProjectNoteEntity { Id = 3L, Name = "Active1" },
            new ProjectNoteEntity { Id = 4L, Name = "Active2" },
        };

        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        repoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        var result = await controller.GetAllActive() as OkObjectResult;

        Assert.IsNotNull(result);
        var models = result!.Value as IEnumerable<ProjectNote>;
        Assert.IsNotNull(models);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), models!.Select(m => m.Id).ToList());

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        var badResult1 = await controller.Update(1L, null as ProjectNoteEntity);
        Assert.IsInstanceOfType(badResult1, typeof(BadRequestResult));

        ProjectNoteEntity updated = new ProjectNoteEntity { Id = 2L, Name = "X" };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((ProjectNoteEntity?)null);

        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        ProjectNoteEntity updated = new ProjectNoteEntity { Id = 5L, Name = "Z" };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNoContent_OnSuccess()
    {
        ProjectNoteEntity existing = new ProjectNoteEntity { Id = 6L, Name = "Before" };

        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.Update(It.IsAny<ProjectNoteEntity>())).Verifiable();
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        ProjectNoteEntity updated = new ProjectNoteEntity { Id = 6L, Name = "After" };
        var result = await controller.Update(6L, updated);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        repoMock.Verify(r => r.Update(It.IsAny<ProjectNoteEntity>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        var result = await controller.Create(null as ProjectNoteEntity);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        ProjectNoteEntity toCreate = new ProjectNoteEntity { Id = 7L, Name = "New" };

        Mock<IGenericRepository<ProjectNoteEntity>> repoMock = new Mock<IGenericRepository<ProjectNoteEntity>>();
        repoMock.Setup(r => r.AddAsync(toCreate, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        ProjectNotesController controller = new ProjectNotesController(repoMock.Object, _mapper);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<ProjectNoteEntity, ProjectNote>.GetById), result!.ActionName);
        var model = result.Value as ProjectNote;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
