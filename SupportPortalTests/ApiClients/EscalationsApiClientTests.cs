using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalUI.ApiClients;
using SupportPortalUI.Models;

namespace SupportPortalTests.ApiClients;

[TestClass]
public class EscalationsApiClientTests
{
    [TestMethod]
    public async Task GetActiveAsync_ReturnsItems()
    {
        var expected = new[] { new EscalationDto { Id = 1, Name = "E1" } };
        var handler = new FakeHandler(req => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expected)
        });

        var http = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
        var client = new EscalationsApiClient(http);

        var items = await client.GetActiveAsync(5);

        Assert.IsNotNull(items);
        CollectionAssert.AreEqual(expected.Select(e => e.Id).ToList(), items.Select(i => i.Id).ToList());
    }

    private class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responder(request));
    }
}
