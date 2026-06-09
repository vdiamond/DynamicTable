using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Table
{
    public class Columns : IColumns
    {
        public IColumn this[string name]
        {
            get { return _columnKey[name]; }
        }
        public IColumn NewColumn(string name, TypeCode typeCode)
        {
            if (name == null)
            {
                throw new Exception("Column name cannot be NULL");
            }
            var ret = Column.CreateColumn(name, typeCode, _rowCount);
            _columnKey.Add(name, ret);
            return ret;
        }
        public bool Remove(IColumn column)
        {
            _columnKey.Remove(column.Name);
            return true;
        }
        public int Count { get { return _columnKey.Count; } }
        public bool ColumnExists(string name)
        {
            return _columnKey.ContainsKey(name);
        }
        public bool Contains(IColumn item)
        {
            if (ColumnExists(item.Name))
            {
                var c = this[item.Name];
                if (c.TypeCode == item.TypeCode && c.Count == item.Count)
                {
                    return true;
                }
            }
            return false;
        }
        public IEnumerator<IColumn> GetEnumerator()
        {
            foreach (var kv in _columnKey)
            {
                yield return kv.Value;
            }
        }
        public void Clear()
        {
            var e = GetEnumerator();
            while (e.MoveNext())
            {
                Remove(e.Current);
            }
        }

        #region ICollection<IColumn> Only
        bool ICollection<IColumn>.IsReadOnly { get { return false; } }
        #endregion
        #region IEnumerable Only
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var kv in _columnKey)
            {
                yield return kv.Value;
            }
        }
        #endregion
        #region ICollection<T> Only
        public void Add(IColumn item)
        {
            throw new NotImplementedException();
        }
        public void CopyTo(IColumn[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region internal
        internal Columns(RowCount rowCount)
        {
            _rowCount = rowCount;
            _columnKey = new Dictionary<string, Column>();
        }
        internal Column GetColumn(string name)
        {
            return _columnKey[name];
        }
        internal void Truncate()
        {
            foreach (var kv in _columnKey)
            {
                kv.Value.Truncate();
            }
        }
        internal void RemoveRows(int index, int count = 1)
        {
            foreach (var kv in _columnKey)
            {
                kv.Value.RemoveRows(index, count);
            }
        }
        #endregion
        #region private
        private RowCount _rowCount;
        private Dictionary<string, Column> _columnKey;
        #endregion
    }
}
