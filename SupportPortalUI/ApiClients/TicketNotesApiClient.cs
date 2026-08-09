using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class TicketNotesApiClient : ITicketNotesApiClient
{
    private readonly HttpClient _http;

    public TicketNotesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<TicketNote>> GetByTicketIdAsync(Int64 ticketId, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<TicketNote>>($"api/ticketnotes?ticketId={ticketId}", cancellationToken);
            return items ?? Array.Empty<TicketNote>();
        }
        catch
        {
            return Array.Empty<TicketNote>();
        }
    }

    public async Task<TicketNote?> CreateAsync(TicketNote note, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/ticketnotes", note, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<TicketNote>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
