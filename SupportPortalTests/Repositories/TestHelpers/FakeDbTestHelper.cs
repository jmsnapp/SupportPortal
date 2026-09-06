using System;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SupportPortalTests.Repositories.TestHelpers
{
    public class FakeDbResources
    {
        public FakeDbCommand Command { get; set; } = null!;

        public FakeDbConnection Connection { get; set; } = null!;

    }

    public static class FakeDbTestHelper
    {
        public static FakeDbResources Create(DbDataReader initialReader, object? initialScalar = null)
        {
            var cmd = new FakeDbCommand(
                readerFactory: ct => Task.FromResult(initialReader),
                scalarFactory: ct => Task.FromResult<object?>(initialScalar)
            );

            var conn = new FakeDbConnection(cmd);
            return new FakeDbResources { Command = cmd, Connection = conn };

        }

        public static void SetReader(FakeDbCommand cmd, DbDataReader reader)
        {
            var readerField = typeof(FakeDbCommand).GetField("_readerFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            if (readerField == null) throw new InvalidOperationException("Cannot find _readerFactory on FakeDbCommand.");
            readerField.SetValue(cmd, new Func<CancellationToken, Task<DbDataReader>>(ct => Task.FromResult(reader)));

        }

        public static void SetScalar(FakeDbCommand cmd, object? scalar)
        {
            var scalarField = typeof(FakeDbCommand).GetField("_scalarFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            if (scalarField == null) throw new InvalidOperationException("Cannot find _scalarFactory on FakeDbCommand.");
            scalarField.SetValue(cmd, new Func<CancellationToken, Task<object?>>(ct => Task.FromResult<object?>(scalar)));

        }

        public static DbParameterCollection GetParameters(FakeDbCommand cmd)
        {
            return cmd.Parameters;

        }

        public static object? GetParameterValue(FakeDbCommand cmd, string parameterName)
        {
            var parms = GetParameters(cmd);
            try
            {
                var p = parms[parameterName];
                return p?.Value;

            }

            catch (IndexOutOfRangeException)
            {
                return null;

            }

        }

    }

}
