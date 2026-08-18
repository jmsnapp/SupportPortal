using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportPortalDomain.Models;
using SupportPortalInfrastructure.Data;

namespace SupportPortalTests.EndToEnd;

/// <summary>
/// Covers DbUpdateExceptionFilter. Every table in this schema carries a unique Name and a web
/// of restricted foreign keys, so duplicate-name and bad-reference are the two most likely
/// client mistakes. Untranslated they surface as 500s, which tells a caller nothing and buries
/// genuine server faults among routine input errors.
/// </summary>
// Shares one database with every other test in this file, so it cannot run under the
// assembly-wide MethodLevel parallelism -- one method's teardown would purge another's row.
[TestClass]
[DoNotParallelize]
public class DbUpdateExceptionFilterTests
{
    private const string Prefix = "ZZTEST_FILT";

    [TestInitialize]
    public void Init() => Api.RequireDatabase();

    [TestCleanup]
    public void Cleanup() => Api.PurgeAsync(Prefix).GetAwaiter().GetResult();

    [TestMethod]
    public async Task DuplicateName_Returns409_NotServerError()
    {
        using HttpClient client = Api.NewClient();
        string name = $"{Prefix}_{Guid.NewGuid():N}".Substring(0, 40);

        HttpResponseMessage first = await client.PostAsJsonAsync("/api/severities",
            new { name, description = "first" });
        Assert.AreEqual(HttpStatusCode.Created, first.StatusCode);

        HttpResponseMessage duplicate = await client.PostAsJsonAsync("/api/severities",
            new { name, description = "second" });

        Assert.AreEqual(HttpStatusCode.Conflict, duplicate.StatusCode,
            "a unique-constraint violation is a client error, not a 500");

        string body = await duplicate.Content.ReadAsStringAsync();
        StringAssert.Contains(body, "Duplicate value");
        StringAssert.Contains(body, "AK_Severities_Name",
            "the response should name the constraint that was violated");
    }

    [TestMethod]
    public async Task NonExistentForeignKey_Returns400_NotServerError()
    {
        using HttpClient client = Api.NewClient();
        SeedIds ids = await GetSeedIdsAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tickets", new
        {
            name = $"{Prefix}_{Guid.NewGuid():N}".Substring(0, 40),
            description = "bad customer reference",
            reproduce = "x",
            resolution = "",
            reportedBy = "tester",
            assignedTo = "tester",
            customer = new { id = 999999L },      // no such customer
            integration = new { id = ids.IntegrationId },
            severity = new { id = ids.SeverityId },
            status = new { id = ids.StatusId }
        });

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode,
            "pointing at a row that does not exist is a client error");

        string body = await response.Content.ReadAsStringAsync();
        StringAssert.Contains(body, "Invalid reference");
        StringAssert.Contains(body, "FK_Tickets_ToCustomer");
    }

    internal sealed record SeedIds(long CustomerId, long IntegrationId, long SeverityId, long StatusId);

    internal static async Task<SeedIds> GetSeedIdsAsync()
    {
        using IServiceScope scope = Api.Factory.Services.CreateScope();
        SupportPortalDBContext ctx = scope.ServiceProvider.GetRequiredService<SupportPortalDBContext>();

        // Id 0 is the DEFAULT sentinel in this design, never a real row.
        long customer = await ctx.Customers.Where(x => !x.Deleted && x.Id > 0).Select(x => x.Id).FirstAsync();
        long integration = await ctx.Integrations.Where(x => !x.Deleted && x.Id > 0).Select(x => x.Id).FirstAsync();
        long severity = await ctx.Severities.Where(x => !x.Deleted && x.Id > 0).Select(x => x.Id).FirstAsync();
        long status = await ctx.SupportStatuses.Where(x => !x.Deleted && x.Id > 0).Select(x => x.Id).FirstAsync();

        return new SeedIds(customer, integration, severity, status);
    }
}
