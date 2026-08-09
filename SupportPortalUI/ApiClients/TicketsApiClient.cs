using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class TicketsApiClient : ITicketsApiClient
{
    private readonly HttpClient _http;

    public TicketsApiClient(HttpClient http)
    {
        _http = http;

    }

    public async Task<IEnumerable<Ticket>> GetActiveAsync(int take = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            List<Ticket>? items = await _http.GetFromJsonAsync<List<Ticket>>("api/tickets/active", cancellationToken);
            return items?.Take(take) ?? Array.Empty<Ticket>();

        }

        catch
        {
            return Array.Empty<Ticket>();

        }

    }

    public async Task<Ticket?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<Ticket>($"api/tickets/{id}", cancellationToken);

        }

        catch
        {
            return null;

        }

    }

    public async Task<bool> UpdateAsync(Int64 id, Ticket ticket, CancellationToken cancellationToken = default)
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
