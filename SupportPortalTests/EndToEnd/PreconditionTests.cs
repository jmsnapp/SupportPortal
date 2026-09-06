using System.Net;
using System.Net.Http.Json;
using SupportPortalDomain.Models;

namespace SupportPortalTests.EndToEnd;

/// <summary>
/// Covers the If-Match precondition on Delete and Restore. Neither carries a body, so without
/// the header they load the current row, find its token matches itself, and always win --
/// meaning a delete could silently discard an edit made a moment earlier.
/// </summary>
// Shares one database with every other test in this file, so it cannot run under the
// assembly-wide MethodLevel parallelism -- one method's teardown would purge another's row.
[TestClass]
[DoNotParallelize]
public class PreconditionTests
{
    private const string Prefix = "ZZTEST_PRE";

    [TestInitialize]
    public void Init() => Api.RequireDatabase();

    [TestCleanup]
    public void Cleanup() => Api.PurgeAsync(Prefix).GetAwaiter().GetResult();

    [TestMethod]
    public async Task Delete_WithSupersededIfMatch_IsRejected_AndTheRowSurvives()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateAsync(client);

        Severity asRead = (await client.GetFromJsonAsync<Severity>(url))!;

        // Somebody else edits the row, superseding the token our caller is holding.
        Severity editor = (await client.GetFromJsonAsync<Severity>(url))!;
        editor.Description = "edited elsewhere";
        Assert.AreEqual(HttpStatusCode.OK, (await client.PutAsJsonAsync(url, editor)).StatusCode);

        HttpResponseMessage deleted = await SendWithIfMatch(client, HttpMethod.Delete, url, asRead.RowVersion);

        Assert.AreEqual(HttpStatusCode.Conflict, deleted.StatusCode,
            "deleting against a superseded version must be refused, not applied");

        Severity actual = (await client.GetFromJsonAsync<Severity>(url))!;
        Assert.IsFalse(actual.Deleted, "the row must still be live");
        Assert.AreEqual("edited elsewhere", actual.Description, "the other edit must survive");
    }

    [TestMethod]
    public async Task Delete_WithCurrentIfMatch_Succeeds()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateAsync(client);

        Severity current = (await client.GetFromJsonAsync<Severity>(url))!;
        HttpResponseMessage deleted = await SendWithIfMatch(client, HttpMethod.Delete, url, current.RowVersion);

        Assert.AreEqual(HttpStatusCode.NoContent, deleted.StatusCode);
        Assert.IsTrue((await client.GetFromJsonAsync<Severity>(url))!.Deleted);
    }

    [TestMethod]
    public async Task Delete_WithoutIfMatch_IsUnguarded()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateAsync(client);

        // Documents that the precondition is opt-in: existing callers keep working unchanged.
        HttpResponseMessage deleted = await client.DeleteAsync(url);

        Assert.AreEqual(HttpStatusCode.NoContent, deleted.StatusCode);
        Assert.IsTrue((await client.GetFromJsonAsync<Severity>(url))!.Deleted);
    }

    [TestMethod]
    public async Task Delete_WithMalformedIfMatch_Returns400()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateAsync(client);

        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.TryAddWithoutValidation("If-Match", "\"not-base64!!\"");
        HttpResponseMessage response = await client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.IsFalse((await client.GetFromJsonAsync<Severity>(url))!.Deleted,
            "a malformed precondition must not fall through to an unguarded delete");
    }

    [TestMethod]
    public async Task Restore_WithSupersededIfMatch_IsRejected()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateAsync(client);

        Severity asRead = (await client.GetFromJsonAsync<Severity>(url))!;

        // Deleting bumps the token, so the copy taken above is now stale.
        Assert.AreEqual(HttpStatusCode.NoContent, (await client.DeleteAsync(url)).StatusCode);

        string restoreUrl = url.Replace("/api/severities/", "/api/severities/restore/");
        HttpResponseMessage restored = await SendWithIfMatch(client, HttpMethod.Put, restoreUrl, asRead.RowVersion);

        Assert.AreEqual(HttpStatusCode.Conflict, restored.StatusCode);
        Assert.IsTrue((await client.GetFromJsonAsync<Severity>(url))!.Deleted, "the row must still be deleted");
    }

    [TestMethod]
    public async Task Restore_WithCurrentIfMatch_Succeeds()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateAsync(client);

        Assert.AreEqual(HttpStatusCode.NoContent, (await client.DeleteAsync(url)).StatusCode);

        Severity current = (await client.GetFromJsonAsync<Severity>(url))!;
        string restoreUrl = url.Replace("/api/severities/", "/api/severities/restore/");
        HttpResponseMessage restored = await SendWithIfMatch(client, HttpMethod.Put, restoreUrl, current.RowVersion);

        Assert.AreEqual(HttpStatusCode.NoContent, restored.StatusCode);
        Assert.IsFalse((await client.GetFromJsonAsync<Severity>(url))!.Deleted);
    }

    private static async Task<HttpResponseMessage> SendWithIfMatch(
        HttpClient client, HttpMethod method, string url, byte[] rowVersion)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.TryAddWithoutValidation("If-Match", $"\"{Convert.ToBase64String(rowVersion)}\"");
        return await client.SendAsync(request);
    }

    private static async Task<string> CreateAsync(HttpClient client)
    {
        var payload = new { name = $"{Prefix}_{Guid.NewGuid():N}".Substring(0, 40), description = "v0" };
        HttpResponseMessage created = await client.PostAsJsonAsync("/api/severities", payload);
        Assert.AreEqual(HttpStatusCode.Created, created.StatusCode);

        Severity model = (await created.Content.ReadFromJsonAsync<Severity>())!;
        return $"/api/severities/{model.Id}";
    }
}
