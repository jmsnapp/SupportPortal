using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalTests.Repositories.TestHelpers
{
    public class FakeDbConnection : DbConnection
    {
        private readonly FakeDbCommand _command;
        private string _connectionString = string.Empty;

        public FakeDbConnection(FakeDbCommand command)
        {
            _command = command;

        }

        public override string ConnectionString { get => _connectionString; set => _connectionString = value; }
        public override string Database => "FakeDB";
        public override string DataSource => "Fake";
        public override string ServerVersion => "1.0";
        private System.Data.ConnectionState _state = System.Data.ConnectionState.Closed;
        public override System.Data.ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { _state = System.Data.ConnectionState.Closed; }
        public override void Open() { _state = System.Data.ConnectionState.Open; }

        protected override DbCommand CreateDbCommand()
        {
            // Set the protected DbConnection property on the command via reflection so tests can run
            var prop = _command.GetType().GetProperty("DbConnection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (prop != null)
            {
                prop.SetValue(_command, this);

            }

            return _command;

        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel) => throw new NotSupportedException();

    }

}
