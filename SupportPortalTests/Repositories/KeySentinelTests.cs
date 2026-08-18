using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Entities;
using SupportPortalInfrastructure.Repositories;

namespace SupportPortalTests.Repositories
{
    [TestClass]
    public class KeySentinelTests
    {
        [TestMethod]
        public void Update_OnIdZero_IsModified_NotAdded()
        {
            var options = new DbContextOptionsBuilder<SupportPortalDBContext>()
                .UseSqlServer("Server=(local);Database=SupportPortalDB;Trusted_Connection=True;")
                .Options;                     // never actually connects

            using var ctx = new SupportPortalDBContext(options);

            var defaultPhase = new PhaseEntity { Id = 0, Name = "DEFAULT", Deleted = true };
            ctx.Phases.Update(defaultPhase);

            Assert.AreEqual(EntityState.Modified, ctx.Entry(defaultPhase).State);
        }

        [TestMethod]
        public void Add_OnSentinelId_IsAdded()
        {
            var options = new DbContextOptionsBuilder<SupportPortalDBContext>()
                .UseSqlServer("Server=(local);Database=SupportPortalDB;Trusted_Connection=True;")
                .Options;                     // never actually connects

            using var ctx = new SupportPortalDBContext(options);

            var created = new PhaseEntity { Name = "NEW" };   // Id == -1 from the ctor
            ctx.Phases.Add(created);

            Assert.AreEqual(EntityState.Added, ctx.Entry(created).State);

        }

    }

} 

