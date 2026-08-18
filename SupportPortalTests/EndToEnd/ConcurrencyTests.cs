using System.Net;
using System.Net.Http.Json;
using SupportPortalDomain.Models;

namespace SupportPortalTests.EndToEnd;

/// <summary>
/// Covers the optimistic concurrency token end to end.
/// <para>
/// This is the highest-value test in the suite because the feature fails *silently*: if the
/// OriginalValue promotion in GenericRepository.Update is removed, every stale write still
/// returns 204 and simply discards the earlier edit. No exception, no bad status code. The
/// only way to notice is to assert on the lost-update scenario, which is what this does.
/// </para>
/// </summary>
// Shares one database with every other test in this file, so it cannot run under the
// assembly-wide MethodLevel parallelism -- one method's teardown would purge another's row.
[TestClass]
[DoNotParallelize]
public class ConcurrencyTests
{
    private const string Prefix = "ZZTEST_CONC";

    [TestInitialize]
    public void Init() => Api.RequireDatabase();

    [TestCleanup]
    public void Cleanup() => Api.PurgeAsync(Prefix).GetAwaiter().GetResult();

    [TestMethod]
    public async Task StaleRowVersion_IsRejected_AndTheFirstWriteSurvives()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateSeverityAsync(client, "v0");

        // Both users load the same row, so both hold the same token.
        Severity userOne = (await client.GetFromJsonAsync<Severity>(url))!;
        Severity userTwo = (await client.GetFromJsonAsync<Severity>(url))!;
        CollectionAssert.AreEqual(userOne.RowVersion, userTwo.RowVersion,
            "both readers should start from the same version");

        // User one saves first and wins.
        userOne.Description = "edited by user one";
        HttpResponseMessage first = await client.PutAsJsonAsync(url, userOne);
        Assert.AreEqual(HttpStatusCode.OK, first.StatusCode);

        // User two saves against the version they loaded, which is now stale.
        userTwo.Description = "edited by user two";
        HttpResponseMessage second = await client.PutAsJsonAsync(url, userTwo);

        Assert.AreEqual(HttpStatusCode.Conflict, second.StatusCode,
            "a write carrying a superseded RowVersion must be rejected, not silently applied");

        Severity actual = (await client.GetFromJsonAsync<Severity>(url))!;
        Assert.AreEqual("edited by user one", actual.Description,
            "the first write must survive; if this reads 'user two' the update was lost");
    }

    [TestMethod]
    public async Task RowVersion_Advances_WhenTheRowActuallyChanges()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateSeverityAsync(client, "v0");

        Severity before = (await client.GetFromJsonAsync<Severity>(url))!;
        before.Description = "changed";
        Assert.AreEqual(HttpStatusCode.OK, (await client.PutAsJsonAsync(url, before)).StatusCode);

        Severity after = (await client.GetFromJsonAsync<Severity>(url))!;
        CollectionAssert.AreNotEqual(before.RowVersion, after.RowVersion,
            "SQL Server must issue a new token whenever the row is written");
    }

    [TestMethod]
    public async Task RowVersion_DoesNotAdvance_WhenNothingChanged()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateSeverityAsync(client, "v0");

        // Guards a real trap: an unmodified round trip produces no UPDATE, so the token does
        // not move. A concurrency test written against an unmodified PUT proves nothing.
        Severity loaded = (await client.GetFromJsonAsync<Severity>(url))!;
        Assert.AreEqual(HttpStatusCode.OK, (await client.PutAsJsonAsync(url, loaded)).StatusCode);

        Severity after = (await client.GetFromJsonAsync<Severity>(url))!;
        CollectionAssert.AreEqual(loaded.RowVersion, after.RowVersion);
    }

    [TestMethod]
    public async Task SavedModel_CarriesTheNewToken_SoConsecutiveSavesNeedNoReRead()
    {
        using HttpClient client = Api.NewClient();
        string url = await CreateSeverityAsync(client, "v0");

        Severity working = (await client.GetFromJsonAsync<Severity>(url))!;
        byte[] tokenAsRead = working.RowVersion;

        // First save. The response body is the point of this test: it is what lets a caller
        // keep editing. With a 204 there would be nothing here and the second save below
        // would replay tokenAsRead and be rejected.
        working.Description = "first edit";
        HttpResponseMessage first = await client.PutAsJsonAsync(url, working);
        Assert.AreEqual(HttpStatusCode.OK, first.StatusCode);

        Severity returned = (await first.Content.ReadFromJsonAsync<Severity>())!;
        CollectionAssert.AreNotEqual(tokenAsRead, returned.RowVersion,
            "the returned model must carry the token the row now holds, not the one that was sent");

        // Second save straight off the returned model, with no intervening GET.
        returned.Description = "second edit";
        HttpResponseMessage second = await client.PutAsJsonAsync(url, returned);
        Assert.AreEqual(HttpStatusCode.OK, second.StatusCode,
            "a caller should be able to save twice in a row without re-reading");

        Severity actual = (await client.GetFromJsonAsync<Severity>(url))!;
        Assert.AreEqual("second edit", actual.Description);

    }

    private static async Task<string> CreateSeverityAsync(HttpClient client, string description)
    {
        var payload = new { name = $"{Prefix}_{Guid.NewGuid():N}".Substring(0, 40), description };
        HttpResponseMessage created = await client.PostAsJsonAsync("/api/severities", payload);
        Assert.AreEqual(HttpStatusCode.Created, created.StatusCode);

        Severity model = (await created.Content.ReadFromJsonAsync<Severity>())!;
        return $"/api/severities/{model.Id}";
    }
}
