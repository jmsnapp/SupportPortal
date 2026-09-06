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

    private DBMapper _mapper = new DBMapper();

    [TestMethod]
    public async Task GetById_ReturnsOk_WhenEntityFound()
    {
        Industry industry = new Industry { Id = 1L, Name = "Mock", Description = "Mock Industry", Deleted = false};
        Customer customer = new Customer { Id = 1L, Name = "Open"};
        customer.Industry = industry;

        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Customer;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);

        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        Industry industry = new Industry { Id = 1L, Name = "Mock", Description = "Mock Industry", Deleted = false };
        Customer customer = new Customer { Id = 1L, Name = "Open" };
        customer.Industry = industry;

        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("Open", It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.GetByName("Open") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Customer;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        CustomerListItem customer1 = new CustomerListItem { Id = 1L, Name = "A", Description = "Mock Customer A", Deleted = false, IndustryId = 1, IndustryDescription = "Mock Industry" };
        CustomerListItem customer2 = new CustomerListItem { Id = 2L, Name = "B", Description = "Mock Customer B", Deleted = false, IndustryId = 1, IndustryDescription = "Mock Industry" };

        List<CustomerListItem> lstCustomers = new List<CustomerListItem>();
        lstCustomers.Add(customer1);
        lstCustomers.Add(customer2);

        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(lstCustomers);

        CustomersController controller = new CustomersController(repoMock.Object);

        ActionResult<PagedResult<CustomerListItem>> result = await controller.GetAll();

        PagedResult<CustomerListItem> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(lstCustomers.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(lstCustomers.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()));

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        CustomerListItem customer1 = new CustomerListItem { Id = 1L, Name = "Active1", Description = "Mock Customer 1", Deleted = false, IndustryId = 1, IndustryDescription = "Mock Industry" };
        CustomerListItem customer2 = new CustomerListItem { Id = 2L, Name = "Active2", Description = "Mock Customer 2", Deleted = false, IndustryId = 1, IndustryDescription = "Mock Industry" };

        List<CustomerListItem> lstCustomers = new List<CustomerListItem>();
        lstCustomers.Add(customer1);
        lstCustomers.Add(customer2);

        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(((List<CustomerListItem>)lstCustomers));

        CustomersController controller = new CustomersController(repoMock.Object);

        ActionResult<PagedResult<CustomerListItem>> result = await controller.GetAllActive();

        PagedResult<CustomerListItem> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(lstCustomers.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(lstCustomers.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()));

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();

        CustomersController controller = new CustomersController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as Customer);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        var updated = new Customer { Id = 2L, Name = "X" };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        Industry industry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };

        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);

        CustomersController controller = new CustomersController(repoMock.Object);

        var updated = new Customer { Id = 5L, Name = "Z", Description = "Mock Customer Z", Deleted = false };
        updated.Industry = industry;

        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        Industry industry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer existing = new Customer { Id = 6L, Name = "Before", Description = "Mock Customer Before", Deleted = false };
        existing.Industry = industry;

        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        CustomersController controller = new CustomersController(repoMock.Object);

        var updated = new Customer { Id = 6L, Name = "After", Description = "Mock Customer Before", Deleted = false };
        updated.Industry = industry;

        var result = await controller.Update(6L, updated);

        Assert.IsNull(result.Result, "PUT should answer with the ID, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>> repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.Create(null as Customer);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        Industry industry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer toCreate = new Customer { Id = 6L, Name = "New", Description = "Mock New Customer", Deleted = false };
        toCreate.Industry = industry;

        // The re-read has to be a distinct instance: Create stamps Id = -1 on the model it is
        // handed, so returning toCreate here would hand the assertion back that -1.
        Customer saved = new Customer { Id = 6L, Name = "New", Description = "Mock New Customer", Deleted = false };
        saved.Industry = industry;

        var repoMock = new Mock<IGenericRepository<Customer, CustomerListItem, CustomerEntity>>();
        // Moq compares a literal argument with Equals, and CustomerEntity does not override it, so a
        // pre-built entity never matches the one Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(6L);
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        CustomersController controller = new CustomersController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<Customer, CustomerListItem, CustomerEntity>.GetById), result!.ActionName);
        var model = result.Value as Customer;
        Assert.IsNotNull(model);
        Assert.AreEqual(6L, model.Id);

    }

}
