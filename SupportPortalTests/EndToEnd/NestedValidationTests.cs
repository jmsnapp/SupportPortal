using System.Net;
using System.Net.Http.Json;
using SupportPortalDomain.Models;

namespace SupportPortalTests.EndToEnd;

/// <summary>
/// Covers SkipNestedPortalObjectValidation. The domain models carry nested references that the
/// mappers read only the Id from, but MVC validates complex properties recursively and the
/// constructors populate them with empty-but-non-null instances. Without the provider, creating
/// a ticket demands a fully populated Customer, Integration, Severity, Status and Escalation --
/// roughly thirty validation errors for data the write path discards.
/// </summary>
// Shares one database with every other test in this file, so it cannot run under the
// assembly-wide MethodLevel parallelism -- one method's teardown would purge another's row.
[TestClass]
[DoNotParallelize]
public class NestedValidationTests
{
    private const string Prefix = "ZZTEST_VAL";

    [TestInitialize]
    public void Init() => Api.RequireDatabase();

    [TestCleanup]
    public void Cleanup() => Api.PurgeAsync(Prefix).GetAwaiter().GetResult();

    [TestMethod]
    public async Task Create_WithForeignKeyIdsOnly_Succeeds()
    {
        using HttpClient client = Api.NewClient();
        var ids = await DbUpdateExceptionFilterTests.GetSeedIdsAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tickets", new
        {
            name = $"{Prefix}_{Guid.NewGuid():N}".Substring(0, 40),
            description = "cannot log in",
            reproduce = "steps",
            resolution = "",
            reportedBy = "tester",
            assignedTo = "tester",
            // References carry an Id and nothing else -- no name, no description, no escalation.
            customer = new { id = ids.CustomerId },
            integration = new { id = ids.IntegrationId },
            severity = new { id = ids.SeverityId },
            status = new { id = ids.StatusId }
        });

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode,
            $"nested references should not be validated as if they were being created: {await response.Content.ReadAsStringAsync()}");

        Ticket created = (await response.Content.ReadFromJsonAsync<Ticket>())!;
        Assert.AreEqual(ids.CustomerId, created.Customer.Id);
        Assert.AreEqual(ids.SeverityId, created.Severity.Id);
        Assert.AreEqual(0L, created.Escalation.Id,
            "an omitted escalation must resolve to the DEFAULT sentinel, not a bogus key");
    }

    [TestMethod]
    public async Task Create_StillValidatesTheTopLevelModel()
    {
        using HttpClient client = Api.NewClient();
        var ids = await DbUpdateExceptionFilterTests.GetSeedIdsAsync();

        // Suppressing nested validation must not suppress the model's own rules.
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/tickets", new
        {
            name = $"{Prefix}_{Guid.NewGuid():N}".Substring(0, 40),
            description = "missing reportedBy",
            reproduce = "steps",
            resolution = "",
            reportedBy = "",
            assignedTo = "tester",
            customer = new { id = ids.CustomerId },
            integration = new { id = ids.IntegrationId },
            severity = new { id = ids.SeverityId },
            status = new { id = ids.StatusId }
        });

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        string body = await response.Content.ReadAsStringAsync();
        StringAssert.Contains(body, "ReportedBy");
        Assert.IsFalse(body.Contains("Customer.Name"),
            "nested references must not appear in the validation errors");
    }

    [TestMethod]
    public async Task Create_StillEnforcesLengthLimits()
    {
        using HttpClient client = Api.NewClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/severities", new
        {
            name = $"{Prefix}_{Guid.NewGuid():N}".Substring(0, 40),
            description = new string('x', 300)    // column and DTO both cap at 255
        });

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode,
            "an over-long value should be rejected by validation, not by the database");
    }
}
