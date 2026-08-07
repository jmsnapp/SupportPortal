using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class TicketsApiClient : ITicketsApiClient
{
    private readonly HttpClient _http;

    public TicketsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<TicketDto>> GetActiveAsync(int take = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<TicketDto>>("api/tickets/active", cancellationToken);
            return items?.Take(take) ?? Array.Empty<TicketDto>();
        }
        catch
        {
            return Array.Empty<TicketDto>();
        }
    }

    public async Task<TicketDto?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<TicketDto>($"api/tickets/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, TicketDto ticket, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/tickets/{id}", ticket, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
