using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class CustomersApiClient : ICustomersApiClient
{
    private readonly HttpClient _http;

    public CustomersApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<Customer  >> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<Customer>>("api/customers/getall", cancellationToken);
            return items ?? Array.Empty<Customer>();
        }
        catch
        {
            return Array.Empty<Customer>();
        }
    }

    public async Task<Customer?> GetByIdAsync(Int64 id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _http.GetFromJsonAsync<Customer>($"api/customers/{id}", cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Customer?> CreateAsync(Customer dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/customers", dto, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Customer>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Int64 id, Customer dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/customers/{id}", dto, cancellationToken);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
