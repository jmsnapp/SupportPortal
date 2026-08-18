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
public class TicketNoteRepositoryTests
{
    private SupportPortalDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SupportPortalDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportPortalDBContext(options);

    }

    [TestMethod]
    public async Task GetByTicketIdAsync_ReturnsOnlyMatchingNotes()
    {
        using var ctx = CreateContext();

        var notes = new[]
        {
            new TicketNoteEntity { TicketId = 0, Note = "X" },
            new TicketNoteEntity { TicketId = 0, Note = "Y" },
            new TicketNoteEntity { TicketId = 0, Note = "Z" }
        };

        await ctx.TicketNotes.AddRangeAsync(notes);
        await ctx.SaveChangesAsync();

        var repo = new GenericRepository<TicketNoteEntity>(ctx);
        var result = await repo.Query().Where(p => p.TicketId == 0).ToListAsync();

        Assert.AreEqual(3, result.Count);
        CollectionAssert.AreEquivalent(new[] { "X", "Y", "Z" }, result.Select(r => r.Note).ToArray());

    }

}
