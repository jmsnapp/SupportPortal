using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            new LinkProjectPhaseEntity { ProjectId = 1L, PhaseId = 1, Phase = new PhaseEntity { Id = 1 }, Percentage = 10 },
            new LinkProjectPhaseEntity { ProjectId = 1L, PhaseId = 2, Phase = new PhaseEntity { Id = 2 }, Percentage = 20 },
            new LinkProjectPhaseEntity { ProjectId = 1L, PhaseId = 3, Phase = new PhaseEntity { Id = 3 }, Percentage = 30 }
        };

        await ctx.LinkProjectPhases.AddRangeAsync(links);
        await ctx.SaveChangesAsync();

        var repo = new LinkProjectPhaseRepository(ctx);
        var result = await repo.Query().Where(p => p.ProjectId == 1).ToListAsync();

        Assert.AreEqual(3, result.Count);
        CollectionAssert.AreEquivalent(new object[] { 1L, 2L, 3L }, result.Select(r => r.PhaseId).ToArray());

    }

}
