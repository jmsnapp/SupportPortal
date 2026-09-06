using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;

namespace SupportPortalTests.Repositories.TestHelpers
{
    public abstract class RepositoryTestFixture
    {
        protected SupportPortalDBContext Context { get; private set; } = null!;
        protected FakeDbResources Fake { get; private set; } = null!;
        protected DbContextOptions<SupportPortalDBContext> Options { get; private set; } = null!;
        private SqliteConnection? _sqliteConnection;

        [TestInitialize]
        public virtual void Initialize()
        {
            // Use SQLite in-memory relational database so repository code that uses DbConnection works.
            var sqliteConn = new SqliteConnection("Data Source=:memory:");
            sqliteConn.Open();
            Options = new DbContextOptionsBuilder<SupportPortalDBContext>()
                          .UseSqlite(sqliteConn)
                          .Options;

            // Save the connection so we can keep it open for the lifetime of the fixture
            _sqliteConnection = sqliteConn;

            Context = new SupportPortalDBContext(Options);

            // Create a default empty reader and fake resources
            var emptyReader = new FakeDbDataReader(new List<List<Dictionary<string, object?>>>());
            Fake = FakeDbTestHelper.Create(emptyReader, null);

            // Replace the relational connection so repository code uses our fakes
            RelationalConnectionReplacer.Replace(Context, Fake.Connection);
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            Context.Dispose();
            if (_sqliteConnection != null)
            {
                _sqliteConnection.Close();
                _sqliteConnection.Dispose();
                _sqliteConnection = null;

            }

        }

        protected void SetReader(DbDataReader reader) => FakeDbTestHelper.SetReader(Fake.Command, reader);
        protected void SetScalar(object? scalar) => FakeDbTestHelper.SetScalar(Fake.Command, scalar);

        protected DbParameterCollection GetParameters() => FakeDbTestHelper.GetParameters(Fake.Command);
        protected object? GetParameterValue(string name) => FakeDbTestHelper.GetParameterValue(Fake.Command, name);

        protected TRepo CreateRepository<TRepo>() where TRepo : class
        {
            var type = typeof(TRepo);
            var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                           .FirstOrDefault(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(SupportPortalDBContext));

            if (ctor == null)
                throw new InvalidOperationException($"No suitable constructor found for {type.FullName}");

            return (TRepo)ctor.Invoke(new object[] { Context });

        }

    }

}
