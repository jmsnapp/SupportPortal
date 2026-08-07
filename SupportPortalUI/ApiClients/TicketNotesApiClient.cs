using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class TicketNotesApiClient : ITicketNotesApiClient
{
    private readonly HttpClient _http;

    public TicketNotesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<TicketNoteDto>> GetByTicketIdAsync(Int64 ticketId, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<TicketNoteDto>>($"api/ticketnotes?ticketId={ticketId}", cancellationToken);
            return items ?? Array.Empty<TicketNoteDto>();
        }
        catch
        {
            return Array.Empty<TicketNoteDto>();
        }
    }

    public async Task<TicketNoteDto?> CreateAsync(TicketNoteDto note, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/ticketnotes", note, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<TicketNoteDto>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
