using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace Table
{
    public class Rows : IRows
    {
        public IRow NewRow()
        {
            _rowCount.adjust();
            foreach (IColumn col in _columns)
            {
                ((Column)col).AddDefault();
            }
            int rowId = _rowCount.LastRowId;
            return new Row(rowId, _columns);
        }
        public IRow this[int rowId]
        {
            get
            {
                return new Row(rowId, _columns);
            }
            set
            {
                throw new Exception("Row cannot be set in DynamicTable");
            }
        }
        public int Count
        {
            get { return _rowCount.Count; }
        }
        public void Clear() 
        {
            _rowCount.Count = 0;
            _columns.Truncate();
        }
        public int IndexOf(IRow item)
        {
            var r = (Row)item;
            return r.RowNo;
        }
        public void RemoveAt(int index) { RemoveRows(index); }
        public bool Remove(IRow item) { RemoveAt(item.RowNo); return true; }
        public void RemoveRows(int index, int count = 1)
        {
            // lets validate
            if (index < 0 || count < 1) { throw new Exception("For RemoveRows, index >=0 and count >=1"); }
            if (index + count > _rowCount.Count) { throw new Exception("For RemoveRows, indexand count out of range"); }
            _columns.RemoveRows(index, count);
            _rowCount.adjust(count * -1);
        }
        public bool Contains(IRow item) 
        { 
            if (item.RowNo >= Count) { return false; }
            if (columns != item.columns) { return false; }
            return true;
        }
        public IEnumerator<IRow> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }
        public IColumns columns { get { return _columns; } }
        public bool IsReadOnly { get { return false; } }

        #region IList<IRow> Only
        void IList<IRow>.Insert(int index, IRow item) { throw new NotSupportedException(); }
        #endregion
        #region ICollection<IRow> Only
        void ICollection<IRow>.Add(IRow item) { throw new Exception("Invalid operation on DynamicTable column"); }
        void ICollection<IRow>.CopyTo(IRow[] array, int arrayIndex) { throw new Exception("Invalid operation on DynamicTable column");}
        #endregion
        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        #region internal
        internal Rows(RowCount rowCount, Columns columns)
        {
            _columns = columns;
            _rowCount = rowCount;
        }
        #endregion
        #region private
        Columns _columns;
        RowCount _rowCount;
        #endregion
    }
 
}
