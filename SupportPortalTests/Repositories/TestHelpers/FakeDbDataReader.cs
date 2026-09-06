using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace SupportPortalTests.Repositories.TestHelpers
{
    internal class FakeDbDataReader : DbDataReader
    {
        private readonly List<List<Dictionary<string, object?>>> _resultSets;
        private int _currentResult = 0;
        private int _currentRow = -1;

        public FakeDbDataReader(List<List<Dictionary<string, object?>>> resultSets)
        {
            _resultSets = resultSets ?? new List<List<Dictionary<string, object?>>>(0);
            _currentResult = 0;
            _currentRow = -1;

        }

        private List<Dictionary<string, object?>> CurrentSet => (_currentResult < _resultSets.Count) ? _resultSets[_currentResult] : new List<Dictionary<string, object?>>();
        private Dictionary<string, object?>? CurrentRow => (_currentRow >= 0 && _currentRow < CurrentSet.Count) ? CurrentSet[_currentRow] : null;

        public override bool HasRows => CurrentSet.Count > 0;

        public override int FieldCount => (CurrentRow != null) ? CurrentRow.Keys.Count : 0;

        public override bool Read()
        {
            if (_currentResult >= _resultSets.Count) return false;
            _currentRow++;
            return _currentRow < CurrentSet.Count;

        }

        public override bool NextResult()
        {
            if (_currentResult + 1 >= _resultSets.Count) return false;
            _currentResult++;
            _currentRow = -1;
            return true;

        }

        public override int GetOrdinal(string name)
        {
            if (CurrentRow == null) throw new IndexOutOfRangeException("No current row");
            int idx = 0;
            foreach (var key in CurrentRow.Keys)
            {
                if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase)) return idx;
                idx++;

            }

            throw new IndexOutOfRangeException($"Column '{name}' not found");

        }

        public override object GetValue(int ordinal)
        {
            if (CurrentRow == null) throw new IndexOutOfRangeException("No current row");
            int idx = 0;
            foreach (var kv in CurrentRow)
            {
                if (idx == ordinal) return kv.Value ?? DBNull.Value;
                idx++;

            }

            throw new IndexOutOfRangeException($"Ordinal {ordinal} out of range");

        }

        public override bool IsDBNull(int ordinal) => GetValue(ordinal) == DBNull.Value;

        public override string GetString(int ordinal) => (string)GetValue(ordinal)!;
        public override long GetInt64(int ordinal) => Convert.ToInt64(GetValue(ordinal));
        public override int GetInt32(int ordinal) => Convert.ToInt32(GetValue(ordinal));
        public override bool GetBoolean(int ordinal) => Convert.ToBoolean(GetValue(ordinal));
        public override DateTime GetDateTime(int ordinal) => Convert.ToDateTime(GetValue(ordinal));

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            var value = GetValue(ordinal);
            if (value == DBNull.Value) return 0;

            var bytes = (byte[])value!;
            int available = Math.Max(0, bytes.Length - (int)dataOffset);
            if (buffer == null) return available;

            int toCopy = Math.Min(available, length);
            Array.Copy(bytes, dataOffset, buffer, bufferOffset, toCopy);
            return toCopy;

        }

        public override string GetName(int ordinal)
        {
            if (CurrentRow == null) throw new IndexOutOfRangeException("No current row");
            int idx = 0;
            foreach (var key in CurrentRow.Keys)
            {
                if (idx == ordinal) return key;
                idx++;

            }

            throw new IndexOutOfRangeException($"Ordinal {ordinal} out of range");

        }

        #region RequiredMembers
        public override int Depth => throw new NotImplementedException();
        public override bool IsClosed => false;
        public override int RecordsAffected => throw new NotImplementedException();
        public override object this[int ordinal] => GetValue(ordinal);
        public override object this[string name] => GetValue(GetOrdinal(name));
        public override IEnumerator GetEnumerator() => throw new NotImplementedException();
        public override int GetValues(object[] values) => throw new NotImplementedException();
        public override byte GetByte(int ordinal) => (byte)GetValue(ordinal)!;
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => throw new NotImplementedException();
        public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal)!;
        public override short GetInt16(int ordinal) => Convert.ToInt16(GetValue(ordinal));
        public override DataTable GetSchemaTable() => throw new NotImplementedException();

        // Async API compatibility
        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) => Task.FromResult(NextResult());
        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => Task.FromResult(Read());

        // Type helpers
        public override Type GetFieldType(int ordinal) => GetValue(ordinal)?.GetType() ?? typeof(object);
        public override double GetDouble(int ordinal) => Convert.ToDouble(GetValue(ordinal));
        public override float GetFloat(int ordinal) => Convert.ToSingle(GetValue(ordinal));
        public override decimal GetDecimal(int ordinal) => Convert.ToDecimal(GetValue(ordinal));
        public override string GetDataTypeName(int ordinal) => GetFieldType(ordinal).Name;
        public override char GetChar(int ordinal) => Convert.ToChar(GetValue(ordinal));

        #endregion

    }

}
