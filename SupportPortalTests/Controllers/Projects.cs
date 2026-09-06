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
public class ProjectsControllerTests
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
        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default Company", Deleted = true};
        newCustomer.Industry = newIndustry;
        Phase newPhase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = false };
        Project entity = new Project { Id = 1L, Name = "DEFAULT", Description = "Default" };
        entity.Customer = newCustomer;
        entity.CurrentPhase = newPhase;

        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        ProjectsController controller = new ProjectsController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Project;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);

        ProjectsController controller = new ProjectsController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default Company", Deleted = true };
        newCustomer.Industry = newIndustry;
        Phase newPhase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Project entity = new Project { Id = 2L, Name = "DEFAULT", Description = "Default", Deleted = true };
        entity.Customer = newCustomer;
        entity.CurrentPhase = newPhase;

        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("DEFAULT", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        ProjectsController controller = new ProjectsController(repoMock.Object);

        var result = await controller.GetByName("DEFAULT") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Project;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("DEFAULT", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default Company", Deleted = true };
        newCustomer.Industry = newIndustry;
        Phase newPhase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Project entity = new Project { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        entity.Customer = newCustomer;
        entity.CurrentPhase = newPhase;

        Project entity2 = new Project { Id = 1L, Name = "STAR2DOW", Description = "STAR Labs to Department of War feed", Deleted = false };
        entity2.Customer = newCustomer;
        entity2.CurrentPhase = newPhase;

        List<Project> entities = new List<Project>();
        entities.Add(entity);
        entities.Add(entity2);

        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        ProjectsController controller = new ProjectsController(repoMock.Object);

        ActionResult<PagedResult<Project>> result = await controller.GetAll();

        PagedResult<Project> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default Company", Deleted = true };
        newCustomer.Industry = newIndustry;
        Phase newPhase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Project entity = new Project { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = false };
        entity.Customer = newCustomer;
        entity.CurrentPhase = newPhase;

        Project entity2 = new Project { Id = 1L, Name = "STAR2DOW", Description = "STAR Labs to Department of War feed", Deleted = false };
        entity2.Customer = newCustomer;
        entity2.CurrentPhase = newPhase;

        List<Project> entities = new List<Project>();
        entities.Add(entity);
        entities.Add(entity2);

        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        ProjectsController controller = new ProjectsController(repoMock.Object);

        ActionResult<PagedResult<Project>> result = await controller.GetAllActive();

        PagedResult<Project> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        ProjectsController controller = new ProjectsController(repoMock.Object);

        var badResult1 = await controller.Update(0L, null as Project);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default Company", Deleted = true };
        newCustomer.Industry = newIndustry;
        Phase newPhase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Project entity = new Project { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        entity.Customer = newCustomer;
        entity.CurrentPhase = newPhase;

        var badResult2 = await controller.Update(1L, entity);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((Project?)null);

        ProjectsController controller = new ProjectsController(repoMock.Object);

        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default Company", Deleted = true };
        newCustomer.Industry = newIndustry;
        Phase newPhase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Project entity = new Project { Id = 5L, Name = "DEFAULT", Description = "Default", Deleted = true };

        var result = await controller.Update(5L, entity);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default Company", Deleted = true };
        newCustomer.Industry = newIndustry;
        Phase newPhase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Project entity = new Project { Id = 6L, Name = "DEFAULT", Description = "Default", Deleted = true };
        entity.Customer = newCustomer;
        entity.CurrentPhase = newPhase;

        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        ProjectsController controller = new ProjectsController(repoMock.Object);

        Project updated = new Project { Id = 6L, Name = "TEST", Description = "Default", Deleted = true };
        entity.Customer = newCustomer;
        entity.CurrentPhase = newPhase;

        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the ID, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        ProjectsController controller = new ProjectsController(repoMock.Object);

        var result = await controller.Create(null as Project);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default Company", Deleted = true };
        newCustomer.Industry = newIndustry;
        Phase newPhase = new Phase { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };

        Project toCreate = new Project { Id = 2L, Name = "TEST", Description = "Test", Deleted = false };
        toCreate.Customer = newCustomer;
        toCreate.CurrentPhase = newPhase;

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        Project saved = new Project { Id = 2L, Name = "TEST", Description = "Test", Deleted = false };
        saved.Customer = newCustomer;
        saved.CurrentPhase = newPhase;

        var repoMock = new Mock<IGenericRepository<Project, Project, ProjectEntity>>();
        // Moq compares a literal argument with Equals, which ProjectEntity does not override,
        // so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(2L);
        repoMock.Setup(r => r.GetByIdAsync(2L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        ProjectsController controller = new ProjectsController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<Project, Project, ProjectEntity>.GetById), result!.ActionName);
        var model = result.Value as Project;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);

    }

}
