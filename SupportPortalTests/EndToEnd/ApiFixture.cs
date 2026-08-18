using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportPortalInfrastructure.Data;

namespace SupportPortalTests.EndToEnd;

/// <summary>
/// Boots the real API pipeline in-process: the real controllers, the real
/// SkipNestedPortalObjectValidation provider, the real DbUpdateExceptionFilter and the real
/// SQL Server. That combination is the point — the in-memory provider used elsewhere in this
/// suite enforces no concurrency tokens, unique constraints, foreign keys or column widths,
/// so none of the behaviour these tests cover can be observed through it.
/// </summary>
internal sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseEnvironment("Development");
}

internal static class Api
{
    private static readonly Lazy<ApiFactory> Lazy = new(() => new ApiFactory());

    public static ApiFactory Factory => Lazy.Value;

    public static HttpClient NewClient() => Factory.CreateClient();

    /// <summary>
    /// These tests write to a real database. On a machine without it, report Inconclusive
    /// rather than Failed — a missing local SQL Server is not a defect in the code.
    /// </summary>
    public static void RequireDatabase()
    {
        try
        {
            using var scope = Factory.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<SupportPortalDBContext>();
            if (!ctx.Database.CanConnect())
                Assert.Inconclusive("SQL Server is not reachable; integration tests skipped.");
        }
        catch (Exception ex)
        {
            Assert.Inconclusive($"SQL Server is not reachable ({ex.GetType().Name}); integration tests skipped.");
        }
    }

    /// <summary>
    /// Hard-deletes anything this fixture created. The API only soft-deletes, so cleanup goes
    /// straight through the context; every test prefixes its rows so nothing else is touched.
    /// <para>
    /// ExecuteDelete rather than load-then-Remove on purpose: a tracked delete carries the
    /// RowVersion in its WHERE clause, so tearing down a row the test just edited fails the
    /// concurrency check. Teardown has no business being optimistically concurrent.
    /// </para>
    /// </summary>
    public static async Task PurgeAsync(string namePrefix)
    {
        using var scope = Factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<SupportPortalDBContext>();

        await ctx.Tickets.IgnoreQueryFilters().Where(x => x.Name.StartsWith(namePrefix)).ExecuteDeleteAsync();
        await ctx.Severities.Where(x => x.Name.StartsWith(namePrefix)).ExecuteDeleteAsync();
        await ctx.Industries.Where(x => x.Name.StartsWith(namePrefix)).ExecuteDeleteAsync();
    }
}
