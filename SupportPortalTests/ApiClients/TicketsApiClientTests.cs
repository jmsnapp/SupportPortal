using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalUI.ApiClients;
using SupportPortalDomain.Models;

namespace SupportPortalTests.ApiClients;

[TestClass]
public class TicketsApiClientTests
{
    [TestMethod]
    public async Task GetByIdAsync_ReturnsTicket()
    {
        var expected = new Ticket { Id = 42, Name = "T42" };
        var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expected)
        });

        var http = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
        var client = new TicketsApiClient(http);

        var item = await client.GetByIdAsync(42);

        Assert.IsNotNull(item);
        Assert.AreEqual(expected.Id, item!.Id);
    }

    private class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responder(request));
    }
}
