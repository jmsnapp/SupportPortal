using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SupportPortalAPI.Controllers;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalTests.Controllers;

[TestClass]
public class TicketsControllerTests
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
        IntegrationStatus newIntegrationStatus = new IntegrationStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationType newType = new IntegrationType { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Integration newIntegration = new Integration { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        SupportStatus newSupportStatus = new SupportStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Severity newSeverity = new Severity { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Escalation newEscalation = new Escalation { Id = 0L, Description = "Default", Deleted = true };
        Ticket newTicket = new Ticket { Id = 1L, Description = "Default", Deleted = true };
        newCustomer.Industry = newIndustry;
        newIntegration.Customer = newCustomer;
        newIntegration.CurrentStatus = newIntegrationStatus;
        newIntegration.Type = newType;
        newTicket.Status = newSupportStatus;
        newTicket.Severity = newSeverity;
        newTicket.Customer = newCustomer;
        newTicket.Escalation = newEscalation;
        newTicket.Integration = newIntegration;

        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(newTicket);

        TicketsController controller = new TicketsController(repoMock.Object);
        var result = await controller.GetById(1L) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Ticket;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Default", model.Description);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((Ticket?)null);

        TicketsController controller = new TicketsController(repoMock.Object);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        TicketListItem newTicket1 = new TicketListItem { Id = 0L, CustomerId = 0, IntegrationId = 0, SeverityId = 0, EscalationId = 0, StatusId = 0, Description = "Default", CustomerDescription = "Default", IntegrationDescription = "Default", SeverityDescription = "Default", EscalationDescription = "Default", StatusDescription = "Default" };
        TicketListItem newTicket2 = new TicketListItem { Id = 1L, CustomerId = 0, IntegrationId = 0, SeverityId = 0, EscalationId = 0, StatusId = 0, Description = "Lex2Doom integration down", CustomerDescription = "Default", IntegrationDescription = "Default", SeverityDescription = "Default", EscalationDescription = "Default", StatusDescription = "Default" };

        List<TicketListItem> entities = new List<TicketListItem>();
        entities.Add(newTicket1);
        entities.Add(newTicket2);

        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        TicketsController controller = new TicketsController(repoMock.Object);

        ActionResult<PagedResult<TicketListItem>> result = await controller.GetAll();

        PagedResult<TicketListItem> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        TicketListItem newTicket1 = new TicketListItem { Id = 0L, CustomerId = 0, IntegrationId = 0, SeverityId = 0, EscalationId = 0, StatusId = 0, Description = "Default", CustomerDescription = "Default", IntegrationDescription = "Default", SeverityDescription = "Default", EscalationDescription = "Default", StatusDescription = "Default" };
        TicketListItem newTicket2 = new TicketListItem { Id = 1L, CustomerId = 0, IntegrationId = 0, SeverityId = 0, EscalationId = 0, StatusId = 0, Description = "Lex2Doom integration down", CustomerDescription = "Default", IntegrationDescription = "Default", SeverityDescription = "Default", EscalationDescription = "Default", StatusDescription = "Default" };

        List<TicketListItem> entities = new List<TicketListItem>();
        entities.Add(newTicket1);
        entities.Add(newTicket2);

        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        repoMock
            .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        TicketsController controller = new TicketsController(repoMock.Object);

        ActionResult<PagedResult<TicketListItem>> result = await controller.GetAllActive();

        PagedResult<TicketListItem> page = result.Value!;
        Assert.IsNotNull(page);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), page.Items.Select(m => m.Id).ToList());
        Assert.AreEqual(entities.Count, page.TotalCount);

        repoMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        TicketsController controller = new TicketsController(repoMock.Object);

        var badResult1 = await controller.Update(1L, null as Ticket);
        Assert.IsInstanceOfType(badResult1.Result, typeof(BadRequestResult));

        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationStatus newIntegrationStatus = new IntegrationStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationType newType = new IntegrationType { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Integration newIntegration = new Integration { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        SupportStatus newSupportStatus = new SupportStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Severity newSeverity = new Severity { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Escalation newEscalation = new Escalation { Id = 0L, Description = "Default", Deleted = true };
        Ticket newTicket = new Ticket { Id = 0L, Description = "Default", Deleted = true };
        newCustomer.Industry = newIndustry;
        newIntegration.Customer = newCustomer;
        newIntegration.CurrentStatus = newIntegrationStatus;
        newIntegration.Type = newType;
        newTicket.Status = newSupportStatus;
        newTicket.Severity = newSeverity;
        newTicket.Customer = newCustomer;
        newTicket.Escalation = newEscalation;
        newTicket.Integration = newIntegration;

        var badResult2 = await controller.Update(1L, newTicket);
        Assert.IsInstanceOfType(badResult2.Result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((Ticket?)null);

        TicketsController controller = new TicketsController(repoMock.Object);

        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationStatus newIntegrationStatus = new IntegrationStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationType newType = new IntegrationType { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Integration newIntegration = new Integration { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        SupportStatus newSupportStatus = new SupportStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Severity newSeverity = new Severity { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Escalation newEscalation = new Escalation { Id = 0L, Description = "Default", Deleted = true };
        Ticket newTicket = new Ticket { Id = 5L, Description = "Default", Deleted = true };
        newCustomer.Industry = newIndustry;
        newIntegration.Customer = newCustomer;
        newIntegration.CurrentStatus = newIntegrationStatus;
        newIntegration.Type = newType;
        newTicket.Status = newSupportStatus;
        newTicket.Severity = newSeverity;
        newTicket.Customer = newCustomer;
        newTicket.Escalation = newEscalation;
        newTicket.Integration = newIntegration;

        var result = await controller.Update(5L, newTicket);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsSavedModel_OnSuccess()
    {
        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationStatus newIntegrationStatus = new IntegrationStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationType newType = new IntegrationType { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Integration newIntegration = new Integration { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        SupportStatus newSupportStatus = new SupportStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Severity newSeverity = new Severity { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Escalation newEscalation = new Escalation { Id = 0L, Description = "Default", Deleted = true };
        Ticket newTicket = new Ticket { Id = 0L, Description = "Default", Deleted = true };
        newCustomer.Industry = newIndustry;
        newIntegration.Customer = newCustomer;
        newIntegration.CurrentStatus = newIntegrationStatus;
        newIntegration.Type = newType;
        newTicket.Status = newSupportStatus;
        newTicket.Severity = newSeverity;
        newTicket.Customer = newCustomer;
        newTicket.Escalation = newEscalation;
        newTicket.Integration = newIntegration;

        TicketEntity updated = new TicketEntity();
        DBMapper.MapTicket2TicketEntity(newTicket, ref updated);

        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(0L, It.IsAny<CancellationToken>())).ReturnsAsync(newTicket);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<TicketEntity>(), It.IsAny<CancellationToken>())).Verifiable();

        TicketsController controller = new TicketsController(repoMock.Object);

        Ticket updatedTicket = new Ticket { Id = 0L, Description = "Test", Deleted = true };
        updatedTicket.Status = newSupportStatus;
        updatedTicket.Severity = newSeverity;
        updatedTicket.Customer = newCustomer;
        updatedTicket.Escalation = newEscalation;
        updatedTicket.Integration = newIntegration;
        var result = await controller.Update(0L, updatedTicket);

        Assert.IsNull(result.Result, "PUT should answer with the ID, not a bare status");

        Assert.IsNotNull(result.Value, "the body carries the refreshed RowVersion so the caller can save again without re-reading");
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<TicketEntity>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        TicketsController controller = new TicketsController(repoMock.Object);

        var result = await controller.Create(null as Ticket);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        Industry newIndustry = new Industry { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationStatus newIntegrationStatus = new IntegrationStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        IntegrationType newType = new IntegrationType { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Customer newCustomer = new Customer { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Integration newIntegration = new Integration { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        SupportStatus newSupportStatus = new SupportStatus { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Severity newSeverity = new Severity { Id = 0L, Name = "DEFAULT", Description = "Default", Deleted = true };
        Escalation newEscalation = new Escalation { Id = 0L, Description = "Default", Deleted = true };
        newCustomer.Industry = newIndustry;
        newIntegration.Customer = newCustomer;
        newIntegration.CurrentStatus = newIntegrationStatus;
        newIntegration.Type = newType;

        Ticket toCreate = new Ticket { Id = 1L, Description = "Test", Deleted = true };
        toCreate.Status = newSupportStatus;
        toCreate.Severity = newSeverity;
        toCreate.Customer = newCustomer;
        toCreate.Escalation = newEscalation;
        toCreate.Integration = newIntegration;

        // Distinct from the request body: Create stamps Id = -1 on the model it is handed.
        Ticket saved = new Ticket { Id = 1L, Description = "Test", Deleted = true };
        saved.Status = newSupportStatus;
        saved.Severity = newSeverity;
        saved.Customer = newCustomer;
        saved.Escalation = newEscalation;
        saved.Integration = newIntegration;

        var repoMock = new Mock<IGenericRepository<Ticket, TicketListItem, TicketEntity>>();
        // Moq compares a literal argument with Equals, which TicketEntity does not override,
        // so only It.IsAny matches the entity Create maps internally.
        repoMock.Setup(r => r.CreateAsync(It.IsAny<TicketEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(1L);
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(saved);

        TicketsController controller = new TicketsController(repoMock.Object);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<Ticket, TicketListItem, TicketEntity>.GetById), result!.ActionName);
        var model = result.Value as Ticket;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);

    }

}
