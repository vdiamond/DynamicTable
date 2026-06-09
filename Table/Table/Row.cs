using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Table
{
    public struct Row : IRow
    {
        public void UpdateFields(params KeyValuePair<string, object>[] values)
        {
            for (int i = 0; i < values.Length; i++ )
            {
                columns[values[i].Key].SetField(_row, values[i].Value);
            }
        }
        public void UpdateFields(KeyValuePair<string, string>[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                 columns[values[i].Key].SetFieldAsString(_row, values[i].Value);
            }
        }
        public T GetField<T>(string name)
        {
            return _columns.GetColumn(name).GetField<T>(_row);
        }
        public string GetFieldAsString(string name)
        {
            return columns[name].GetFieldAsString(_row);
        }
        public object GetFieldAsObject(string name)
        {
            return columns[name].GetField(_row);
        }
        public void SetField<T>(string name, T value)
        {
            _columns.GetColumn(name).SetField<T>(_row, value);
        }
        public void SetFieldAsString(string name, string value)
        {
           columns[name].SetFieldAsString(_row, value);
        }
        public void SetFieldAsObject(string name, object value)
        {
           columns[name].SetField(_row, value);
        }
        public int Count
        {
            get { return _columns.Count; }
        }
        public IEnumerator GetEnumerator()
        {
            foreach (IColumn c in _columns)
            {
                yield return GetFieldAsObject(c.Name);
            }
        }
        public IColumns columns { get { return _columns; } }
        public int RowNo { get { return _row; } }
        #region ICollection Only
        bool ICollection.IsSynchronized {  get { return false; } }
        object ICollection.SyncRoot { get { throw new NotSupportedException(); } }
        void ICollection.CopyTo(Array array, int index) { throw new NotSupportedException();  }
        #endregion
        #region internal
        internal Row(int row, Columns columns) 
        {
            _row = row;
            _columns = columns;
        }
        internal void SetRow(int row)
        {
            _row = row;
        }
        #endregion
        #region private
        private int _row;
        private Columns _columns;
        #endregion
    }
}
