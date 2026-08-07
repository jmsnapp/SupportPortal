using System.Net.Http.Json;
using SupportPortalUI.ApiClients.Interfaces;
using SupportPortalUI.Models;

namespace SupportPortalUI.ApiClients;

public sealed class ProjectNotesApiClient : IProjectNotesApiClient
{
    private readonly HttpClient _http;

    public ProjectNotesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<ProjectNoteDto>> GetByProjectIdAsync(Int64 projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _http.GetFromJsonAsync<IEnumerable<ProjectNoteDto>>($"api/projectnotes?projectId={projectId}", cancellationToken);
            return items ?? Array.Empty<ProjectNoteDto>();
        }
        catch
        {
            return Array.Empty<ProjectNoteDto>();
        }
    }

    public async Task<ProjectNoteDto?> CreateAsync(ProjectNoteDto note, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/projectnotes", note, cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ProjectNoteDto>(cancellationToken: cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
