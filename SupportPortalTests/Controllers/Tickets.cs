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
public class TicketsControllerTests
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
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };
        TicketEntity entity = new TicketEntity { Id = 1L, Name = "Open", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        TicketsController controller = new TicketsController(repoMock.Object, 
                                                             customerRepoMock.Object, 
                                                             integrationRepoMock.Object, 
                                                             escalationRepoMock.Object, 
                                                             severityRepoMock.Object, 
                                                             industryRepoMock.Object, 
                                                             integrationTypeRepoMock.Object, 
                                                             integrationStatusRepoMock.Object, 
                                                             supportStatusRepoMock.Object, 
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);
        var result = await controller.GetById(1) as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Ticket;
        Assert.IsNotNull(model);
        Assert.AreEqual(1L, model.Id);
        Assert.AreEqual("Open", model.Name);

    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenEntityMissing()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(99L, It.IsAny<CancellationToken>())).ReturnsAsync((TicketEntity?)null);

        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        var result = await controller.GetById(99L);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task GetByName_ReturnsOk_WhenFound()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };
        TicketEntity entity = new TicketEntity { Id = 2L, Name = "Closed", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        repoMock.Setup(r => r.GetByNameAsync("Closed", It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        var result = await controller.GetByName("Closed") as OkObjectResult;

        Assert.IsNotNull(result);
        var model = result!.Value as Ticket;
        Assert.IsNotNull(model);
        Assert.AreEqual(2L, model.Id);
        Assert.AreEqual("Closed", model.Name);

    }

    [TestMethod]
    public async Task GetAll_ReturnsMappedList()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };
        List<TicketEntity> entities = new List<TicketEntity>
        {
            new TicketEntity { Id = 1L, Name = "A", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L },
            new TicketEntity { Id = 2L, Name = "B", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L },
        };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.IsNotNull(result);
        var models = result!.Value as IEnumerable<Ticket>;
        Assert.IsNotNull(models);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), models!.Select(m => m.Id).ToList());

    }

    [TestMethod]
    public async Task GetAllActive_ReturnsMappedList()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };
        List<TicketEntity> entities = new List<TicketEntity>
        {
            new TicketEntity { Id = 3L, Name = "Active1", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L },
            new TicketEntity { Id = 4L, Name = "Active2", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L },
        };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        repoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        var result = await controller.GetAllActive() as OkObjectResult;

        Assert.IsNotNull(result);
        var models = result!.Value as IEnumerable<Ticket>;
        Assert.IsNotNull(models);
        CollectionAssert.AreEquivalent(entities.Select(e => e.Id).ToList(), models!.Select(m => m.Id).ToList());

    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_OnNullOrIdMismatch()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        var badResult1 = await controller.Update(1L, null as TicketEntity);
        Assert.IsInstanceOfType(badResult1, typeof(BadRequestResult));

        TicketEntity updated = new TicketEntity { Id = 2L, Name = "X" };
        var badResult2 = await controller.Update(1L, updated);
        Assert.IsInstanceOfType(badResult2, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNotFound_WhenExistingMissing()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(5L, It.IsAny<CancellationToken>())).ReturnsAsync((TicketEntity?)null);

        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        TicketEntity updated = new TicketEntity { Id = 5L, Name = "Z", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L };
        var result = await controller.Update(5L, updated);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));

    }

    [TestMethod]
    public async Task Update_ReturnsNoContent_OnSuccess()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };
        TicketEntity existing = new TicketEntity { Id = 6L, Name = "Before", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        repoMock.Setup(r => r.GetByIdAsync(6L, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        repoMock.Setup(r => r.Update(It.IsAny<TicketEntity>())).Verifiable();
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        TicketEntity updated = new TicketEntity { Id = 6L, Name = "After", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L };
        var result = await controller.Update(6L, updated);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        repoMock.Verify(r => r.Update(It.IsAny<TicketEntity>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNull()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        var result = await controller.Create(null as TicketEntity);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));

    }

    [TestMethod]
    public async Task Create_ReturnsCreatedAtAction_OnSuccess()
    {
        EscalationEntity escalationEntity = new EscalationEntity { Id = 1L, Name = "Escalation1" };
        IndustryEntity industryEntity = new IndustryEntity { Id = 1L, Name = "DEFAULT" };
        CustomerEntity customerEntity = new CustomerEntity { Id = 1L, Name = "Customer1", IndustryId = 1L };
        IntegrationStatusEntity integrationStatusEntity = new IntegrationStatusEntity { Id = 1L, Name = "Status1" };
        IntegrationTypeEntity integrationTypeEntity = new IntegrationTypeEntity { Id = 1L, Name = "Type1" };
        IntegrationEntity integrationEntity = new IntegrationEntity { Id = 1L, Name = "Integration1", CurrentStatusId = 1L, IntegrationTypeId = 1L, CustomerId = 1L };
        SeverityEntity severityEntity = new SeverityEntity { Id = 1L, Name = "Severity1" };
        SupportStatusEntity supportStatusEntity = new SupportStatusEntity { Id = 1L, Name = "SupportStatus1" };
        TicketNoteEntity ticketNoteEntity = new TicketNoteEntity { Id = 1L, TicketId = 1L, Note = "Note1" };
        TicketEntity toCreate = new TicketEntity { Id = 7L, Name = "New", CustomerId = 1L, EscalationId = 1L, IntegrationId = 1L, SeverityId = 1L, StatusId = 1L };

        Mock<IGenericRepository<EscalationEntity>> escalationRepoMock = new Mock<IGenericRepository<EscalationEntity>>();
        escalationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(escalationEntity);

        Mock<IGenericRepository<IndustryEntity>> industryRepoMock = new Mock<IGenericRepository<IndustryEntity>>();
        industryRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(industryEntity);

        Mock<IGenericRepository<CustomerEntity>> customerRepoMock = new Mock<IGenericRepository<CustomerEntity>>();
        customerRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(customerEntity);

        Mock<IGenericRepository<IntegrationStatusEntity>> integrationStatusRepoMock = new Mock<IGenericRepository<IntegrationStatusEntity>>();
        integrationStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationStatusEntity);

        Mock<IGenericRepository<IntegrationTypeEntity>> integrationTypeRepoMock = new Mock<IGenericRepository<IntegrationTypeEntity>>();
        integrationTypeRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationTypeEntity);

        Mock<IGenericRepository<IntegrationEntity>> integrationRepoMock = new Mock<IGenericRepository<IntegrationEntity>>();
        integrationRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(integrationEntity);

        Mock<IGenericRepository<SeverityEntity>> severityRepoMock = new Mock<IGenericRepository<SeverityEntity>>();
        severityRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(severityEntity);

        Mock<IGenericRepository<SupportStatusEntity>> supportStatusRepoMock = new Mock<IGenericRepository<SupportStatusEntity>>();
        supportStatusRepoMock.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(supportStatusEntity);

        Mock<ITicketNoteRepository> ticketNoteRepoMock = new Mock<ITicketNoteRepository>();
        ticketNoteRepoMock.Setup(r => r.GetByTicketIdAsync(1L, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TicketNoteEntity> { ticketNoteEntity });

        Mock<IGenericRepository<TicketEntity>> repoMock = new Mock<IGenericRepository<TicketEntity>>();
        repoMock.Setup(r => r.AddAsync(toCreate, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        TicketsController controller = new TicketsController(repoMock.Object,
                                                             customerRepoMock.Object,
                                                             integrationRepoMock.Object,
                                                             escalationRepoMock.Object,
                                                             severityRepoMock.Object,
                                                             industryRepoMock.Object,
                                                             integrationTypeRepoMock.Object,
                                                             integrationStatusRepoMock.Object,
                                                             supportStatusRepoMock.Object,
                                                             ticketNoteRepoMock.Object,
                                                             _mapper);

        var result = await controller.Create(toCreate) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(GenericController<TicketEntity, Ticket>.GetById), result!.ActionName);
        var model = result.Value as Ticket;
        Assert.IsNotNull(model);
        Assert.AreEqual(7, model.Id);

    }

}
