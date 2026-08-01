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
public class GenericRepositoryTests
{
    private SupportPortalDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SupportPortalDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportPortalDBContext(options);

    }

    [TestMethod]
    public async Task Add_GetAll_GetById_Update_Remove_SaveChanges_Works()
    {
        using var ctx = CreateContext();

        var repo = new GenericRepository<ProjectNoteEntity>(ctx);

        var entity = new ProjectNoteEntity { ProjectId = 0, Note = "initial" };
        await repo.AddAsync(entity);
        await repo.SaveChangesAsync();

        var all = (await repo.GetAllAsync()).ToList();
        Assert.AreEqual(1, all.Count);
        var saved = all.First();

        var byId = await repo.GetByIdAsync(saved.Id);
        Assert.IsNotNull(byId);
        Assert.AreEqual("initial", byId!.Note);

        // Update
        byId.Note = "updated";
        repo.Update(byId);
        await repo.SaveChangesAsync();

        var updated = (await repo.GetByIdAsync(byId.Id))!;
        Assert.AreEqual("updated", updated.Note);

        // Soft-delete handling via Deleted flag: GetAllActive should exclude deleted items
        updated.Deleted = true;
        repo.Update(updated);
        await repo.SaveChangesAsync();

        var active = (await repo.GetAllActiveAsync()).ToList();
        Assert.IsFalse(active.Any());

        // Remove permanently
        repo.Remove(updated);
        await repo.SaveChangesAsync();

        var remaining = (await repo.GetAllAsync()).ToList();
        Assert.AreEqual(0, remaining.Count);

    }

}
