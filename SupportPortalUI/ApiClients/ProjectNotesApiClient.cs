using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalDomain.Models;

namespace SupportPortalUI.ApiClients;

public sealed class ProjectNotesApiClient : IProjectNotesApiClient
{
    private readonly HttpClient _http;

    public ProjectNotesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ProjectNote>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<ProjectNote>>($"api/projectnotes?projectId={projectId}", cancellationToken);
            return items ?? Array.Empty<ProjectNote>();
        }
        catch
        {
            return Array.Empty<ProjectNote>();
        }
    }

    public async Task<ProjectNote?> CreateAsync(ProjectNote note, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/projectnotes", note, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ProjectNote>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
