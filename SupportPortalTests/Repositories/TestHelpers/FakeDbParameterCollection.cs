using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace SupportPortalTests.Repositories.TestHelpers
{
    public class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _inner = new List<DbParameter>();

        public override int Add(object value)
        {
            _inner.Add((DbParameter)value);
            return _inner.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var v in values) _inner.Add((DbParameter)v!);
        }

        public override void Clear() => _inner.Clear();
        public override bool Contains(string value) => _inner.Exists(p => p.ParameterName == value);
        public override int IndexOf(string parameterName) => _inner.FindIndex(p => p.ParameterName == parameterName);
        public override void Insert(int index, object value) => _inner.Insert(index, (DbParameter)value);
        public override void Remove(object value) => _inner.Remove((DbParameter)value);
        public override void RemoveAt(string parameterName) => _inner.RemoveAt(IndexOf(parameterName));
        public override void RemoveAt(int index) => _inner.RemoveAt(index);
        public override int Count => _inner.Count;
        public override object SyncRoot => ((ICollection)_inner).SyncRoot;
        public override bool IsFixedSize => false;
        public override bool IsReadOnly => false;
        public override bool IsSynchronized => false;

        public override int IndexOf(object value) => _inner.IndexOf((DbParameter)value);
        public override bool Contains(object value) => _inner.Contains((DbParameter)value);
        public override void CopyTo(System.Array array, int index) => _inner.ToArray().CopyTo(array, index);
        public override IEnumerator GetEnumerator() => _inner.GetEnumerator();

        protected override DbParameter GetParameter(int index) => _inner[index];
        protected override DbParameter GetParameter(string parameterName) => _inner.Find(p => p.ParameterName == parameterName)!;
        protected override void SetParameter(int index, DbParameter value) => _inner[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var idx = IndexOf(parameterName);
            if (idx >= 0) _inner[idx] = value; else _inner.Add(value);
        }
    }
}
