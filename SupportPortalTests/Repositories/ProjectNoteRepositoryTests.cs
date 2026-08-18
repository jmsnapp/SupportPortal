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
public class ProjectNoteRepositoryTests
{
    private SupportPortalDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SupportPortalDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportPortalDBContext(options);

    }

    [TestMethod]
    public async Task GetByProjectIdAsync_ReturnsOnlyMatchingNotes()
    {
        using var ctx = CreateContext();

        var notes = new[]
        {
            new ProjectNoteEntity { ProjectId = 0, Note = "A" },
            new ProjectNoteEntity { ProjectId = 0, Note = "B" },
            new ProjectNoteEntity { ProjectId = 0, Note = "C" }
        };

        await ctx.ProjectNotes.AddRangeAsync(notes);
        await ctx.SaveChangesAsync();

        var repo = new GenericRepository<ProjectNoteEntity>(ctx);
        var result = await repo.Query().Where(p => p.ProjectId == 0).ToListAsync();

        Assert.AreEqual(3, result.Count);
        CollectionAssert.AreEquivalent(new[] { "A", "B", "C" }, result.Select(r => r.Note).ToArray());

    }

}
