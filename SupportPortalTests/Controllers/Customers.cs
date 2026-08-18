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
public class CustomersControllerTests
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
        CustomerEntity entity = new CustomerEntity { Id = 1L, Name = "Open", IndustryId = 1L };

        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Customer;
        Assert.IsNotNull(model);
        Assert.AreEqual(1, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((CustomerEntity?)null);

        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        CustomerEntity entity = new CustomerEntity { Id = 2L, Name = "Closed", IndustryId = 1L, Deleted = false };

        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("Closed", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.GetByName("Closed") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Customer;
        Assert.IsNotNull(model);
        Assert.AreEqual(2, model.Id);
        Assert.AreEqual("Closed", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        List<CustomerEntity> entities = new List<CustomerEntity>
        {
            new CustomerEntity { Id = 1L, Name = "A", IndustryId = 1L },
            new CustomerEntity { Id = 2L, Name = "B", IndustryId = 1L }
        };

        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        repoMock
            .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<CustomerEntity>)entities, entities.Count));

        CustomersController controller = new CustomersController(repoMock.Object);

        ActionResult<PagedResult<Customer>> result = await controller.GetAll();

        PagedResult<Customer> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetPageAsync(0, 50, true, It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        List<CustomerEntity> entities = new List<CustomerEntity>
        {
            new CustomerEntity { Id = 3L, Name = "Active1", IndustryId = 1L },
            new CustomerEntity { Id = 4L, Name = "Active2", IndustryId = 1L }
        };

        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        repoMock
            .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<CustomerEntity>)entities, entities.Count));

        CustomersController controller = new CustomersController(repoMock.Object);

        ActionResult<PagedResult<Customer>> result = await controller.GetAllActive();

        PagedResult<Customer> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetPageAsync(0, 50, false, It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();

        CustomersController controller = new CustomersController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as Customer);
        Assert.IsInstanceOfType(badResult1, typeof(BadRequestResult));

        var entity = new CustomerEntity { Id = 2L, Name = "X" };
        Customer updated = DBMapper.MapCustomerEntity2Customer(entity);
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2, typeof(BadRequestResult));
    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((CustomerEntity?)null);

        CustomersController controller = new CustomersController(repoMock.Object);

        var entity = new CustomerEntity { Id = 5L, Name = "Z", IndustryId = 1L };
        Customer updated = DBMapper.MapCustomerEntity2Customer(entity);
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNoContent_OnSuccess()
    {
        CustomerEntity existing = new CustomerEntity { Id = 6L, Name = "Before", IndustryId = 1L };

        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.Update(It.IsAny<CustomerEntity>())).Verifiable();
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        CustomersController controller = new CustomersController(repoMock.Object);

        var entity = new CustomerEntity { Id = 6L, Name = "After", IndustryId = 1L };
        Customer updated = DBMapper.MapCustomerEntity2Customer(entity);
        var result = await controller.Update(6L, updated);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        repoMock.Verify(r => r.Update(It.IsAny<CustomerEntity>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.Create(null as Customer);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        CustomerEntity toCreate = new CustomerEntity { Id = 7L, Name = "New", IndustryId = 1L };
        Customer objCreate = DBMapper.MapCustomerEntity2Customer(toCreate);

        Mock<IGenericRepository<CustomerEntity>> repoMock = new Mock<IGenericRepository<CustomerEntity>>();
        repoMock.Setup(r => r.AddAsync(toCreate, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repoMock.Setup(r => r.GetByIdAsync(-1L, It.IsAny<CancellationToken>())).ReturnsAsync(toCreate);

        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.Create(objCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<CustomerEntity, Customer>.GetById), result!.ActionName);
        var model = result.Value as Customer;
        Assert.IsNotNull(model);
        Assert.AreEqual(7L, model.Id);

    }

}
