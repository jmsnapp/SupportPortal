using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;

namespace SupportPortalTests.Repositories
{
    /// <summary>
    /// The tables in this schema seed IDENTITY at 0, so 0 is a real key -- it is the DEFAULT
    /// sentinel row every required foreign key points at. EF's usual "unset key" marker is
    /// default(T), which would be 0, so PortalEntityConfiguration moves it to -1 and the entity
    /// constructor matches. Those two values have to agree: if they drift, EF stops recognising
    /// a new entity and starts emitting explicit identity inserts, which SQL Server rejects.
    /// </summary>
    [TestClass]
    public class KeySentinelTests
    {
        /// <summary>Model-only; these never open a connection.</summary>
        private static SupportPortalDBContext CreateContext()
        {
            DbContextOptions<SupportPortalDBContext> options =
                new DbContextOptionsBuilder<SupportPortalDBContext>()
                    .UseSqlServer("Server=(local);Database=SupportPortalDB;Trusted_Connection=True;")
                    .Options;

            return new SupportPortalDBContext(options);

        }

        [TestMethod]
        public void ConfiguredSentinel_MatchesTheEntityConstructorDefault()
        {
            using SupportPortalDBContext ctx = CreateContext();

            IProperty id = ctx.Model
                              .FindEntityType(typeof(PhaseEntity))!
                              .FindProperty(nameof(PortalEntity.Id))!;

            Assert.AreEqual(new PhaseEntity().Id, id.Sentinel,
                "PortalEntity's constructor and HasSentinel() must agree, or EF cannot tell a " +
                "new entity from an existing one.");

        }

        [TestMethod]
        public void Update_WithSentinelKey_IsAdded()
        {
            using SupportPortalDBContext ctx = CreateContext();

            // Update() is the call that consults the sentinel. Add() marks an entity Added
            // whatever its key holds, so asserting on Add() would pass no matter what the
            // sentinel was configured to -- it cannot fail, and so it cannot protect anything.
            PhaseEntity created = new PhaseEntity { Name = "NEW" };   // Id == sentinel from the ctor
            ctx.Phases.Update(created);

            Assert.AreEqual(EntityState.Added, ctx.Entry(created).State);

        }

        [TestMethod]
        public void Update_OnIdZero_IsModified_NotAdded()
        {
            using SupportPortalDBContext ctx = CreateContext();

            // The case that forced the sentinel off default(long): Id 0 is the DEFAULT row, so
            // it must be treated as an existing key, not as "unset".
            PhaseEntity defaultPhase = new PhaseEntity { Id = 0, Name = "DEFAULT", Deleted = true };
            ctx.Phases.Update(defaultPhase);

            Assert.AreEqual(EntityState.Modified, ctx.Entry(defaultPhase).State);

        }

        [TestMethod]
        public void Update_OnRealKey_IsModified()
        {
            using SupportPortalDBContext ctx = CreateContext();

            PhaseEntity existing = new PhaseEntity { Id = 7, Name = "TESTING" };
            ctx.Phases.Update(existing);

            Assert.AreEqual(EntityState.Modified, ctx.Entry(existing).State);

        }

    }

}
