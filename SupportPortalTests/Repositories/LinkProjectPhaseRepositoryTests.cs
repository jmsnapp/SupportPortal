using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalTests.Repositories;

[TestClass]
public class LinkProjectPhaseRepositoryTests
{
    private SupportPortalDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SupportPortalDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportPortalDBContext(options);

    }

    [TestMethod]
    public async Task GetByProjectIdAsync_ReturnsOnlyMatchingLinks()
    {
        using var ctx = CreateContext();

        var links = new[]
        {
            new LinkProjectPhaseEntity { ProjectId = 0, PhaseId = 1, Percentage = 10 },
            new LinkProjectPhaseEntity { ProjectId = 0, PhaseId = 2, Percentage = 20 },
            new LinkProjectPhaseEntity { ProjectId = 0, PhaseId = 3, Percentage = 30 }
        };

        await ctx.LinkProjectPhases.AddRangeAsync(links);
        await ctx.SaveChangesAsync();

        var repo = new LinkProjectPhaseRepository(ctx);
        var result = (await repo.GetByProjectIdAsync(0)).ToList();

        Assert.AreEqual(3, result.Count);
        CollectionAssert.AreEquivalent(new object[] { 1L, 2L, 3L }, result.Select(r => r.PhaseId).ToArray());

    }

}
