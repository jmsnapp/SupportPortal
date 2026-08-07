using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalUI.ApiClients;
using SupportPortalUI.Models;

namespace SupportPortalTests.ApiClients;

[TestClass]
public class TicketNotesApiClientTests
{
    [TestMethod]
    public async Task CreateAsync_ReturnsCreatedNote()
    {
        var input = new TicketNoteDto { TicketId = 1, Author = "A", Text = "note" };
        var returned = new TicketNoteDto { Id = 100, TicketId = 1, Author = "A", Text = "note", CreatedDate = DateTime.UtcNow };

        var handler = new FakeHandler(req =>
        {
            if (req.Method == HttpMethod.Post && req.RequestUri!.AbsolutePath.Contains("ticketnotes"))
            {
                return new HttpResponseMessage(HttpStatusCode.Created) { Content = JsonContent.Create(returned) };
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        var http = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
        var client = new TicketNotesApiClient(http);

        var result = await client.CreateAsync(input);

        Assert.IsNotNull(result);
        Assert.AreEqual(returned.Id, result!.Id);
    }

    private class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responder(request));
    }
}
