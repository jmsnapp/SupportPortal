using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalTests.Repositories.TestHelpers
{
    public class FakeDbCommand : DbCommand
    {
        private readonly Func<CancellationToken, Task<DbDataReader>> _readerFactory;
        private readonly Func<CancellationToken, Task<object?>> _scalarFactory;
        private readonly DbParameterCollection _parameters = new FakeDbParameterCollection();

        public FakeDbCommand(Func<CancellationToken, Task<DbDataReader>> readerFactory,
                             Func<CancellationToken, Task<object?>> scalarFactory)
        {
            _readerFactory = readerFactory;
            _scalarFactory = scalarFactory;

        }

        public override string CommandText { get; set; } = string.Empty;
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        protected override DbConnection? DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => _parameters;
        protected override DbTransaction? DbTransaction { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }

        public override void Cancel() { }
        public override int ExecuteNonQuery() => 0;
        public override object? ExecuteScalar() => throw new NotSupportedException("Use ExecuteScalarAsync in tests");
        public override void Prepare() { }

        protected override DbParameter CreateDbParameter() => new FakeDbParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new NotSupportedException("Use ExecuteReaderAsync in tests");

        }

        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            return _readerFactory(cancellationToken);

        }

        //// Hide ExecuteReaderAsync to avoid signature differences across framework versions
        //public new Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
        //{
        //    return _readerFactory(cancellationToken);

        //}

        public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return _scalarFactory(cancellationToken);

        }

    }

}
