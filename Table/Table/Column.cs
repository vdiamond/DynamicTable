using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Table
{
    public abstract class Column : IColumn 
    {
        public string Name
        {
            get { return _name; }
        }
        public TypeCode TypeCode
        {
            get { return _typeCode; }
        }
        public T GetField<T>(int rowId)
        {
            return Get<T>(rowId);
        }
        public object GetField(int rowId)
        {
            return GetObject(rowId);
        }
        public string GetFieldAsString(int rowId)
        {
            switch (TypeCode)
            {
                case TypeCode.Boolean: return Get<bool>(rowId).ToString();
                case TypeCode.Byte: return Get<byte>(rowId).ToString();
                case TypeCode.Char: return Get<char>(rowId).ToString();
                case TypeCode.DateTime: return Get<DateTime>(rowId).ToString();
                case TypeCode.Decimal: return Get<Decimal>(rowId).ToString();
                case TypeCode.Double: return Get<double>(rowId).ToString();
                case TypeCode.Int16: return Get<Int16>(rowId).ToString();
                case TypeCode.Int32: return Get<Int32>(rowId).ToString();
                case TypeCode.Int64: return Get<Int64>(rowId).ToString();
                case TypeCode.SByte: return Get<SByte>(rowId).ToString();
                case TypeCode.Single: return Get<Single>(rowId).ToString();
                case TypeCode.String: return Get<String>(rowId).ToString();
                case TypeCode.UInt16: return Get<UInt16>(rowId).ToString();
                case TypeCode.UInt32: return Get<UInt32>(rowId).ToString();
                case TypeCode.UInt64: return Get<UInt64>(rowId).ToString();
            }
            return null;

        }
        public void SetField<T>(int rowId, T value)
        {
            Set<T>(rowId, value); 
        }
        public void SetField(int rowId, object value)
        {
            SetObject(rowId, value);    
        }
        public void SetFieldAsString(int rowId, string value)
        {
            switch (TypeCode)
            {
                case TypeCode.Boolean: Set<bool>(rowId, bool.Parse(value)); break;
                case TypeCode.Byte: Set<byte>(rowId, byte.Parse(value)); break;
                case TypeCode.Char: Set<char>(rowId, char.Parse(value)); break;
                case TypeCode.DateTime: Set<DateTime>(rowId, DateTime.Parse(value)); break;
                case TypeCode.Decimal: Set<Decimal>(rowId, Decimal.Parse(value)); break; 
                case TypeCode.Double: Set<double>(rowId, double.Parse(value)); break; 
                case TypeCode.Int16: Set<Int16>(rowId, Int16.Parse(value)); break;
                case TypeCode.Int32: Set<Int32>(rowId, Int32.Parse(value)); break;
                case TypeCode.Int64: Set<Int64>(rowId, Int64.Parse(value)); break;
                case TypeCode.SByte: Set<SByte>(rowId, SByte.Parse(value)); break;
                case TypeCode.Single: Set<Single>(rowId, Single.Parse(value)); break;
                case TypeCode.String: Set<String>(rowId, value); break;
                case TypeCode.UInt16: Set<UInt16>(rowId, UInt16.Parse(value)); break;
                case TypeCode.UInt32: Set<UInt32>(rowId, UInt32.Parse(value)); break;
                case TypeCode.UInt64: Set<UInt64>(rowId, UInt64.Parse(value)); break;
            }
            return;
        }
        public int Count
        {
            get { return _rowCount.Count; }
        }
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this.GetField(i);
            }
        }

        #region IList only
        bool IList.IsFixedSize {  get { return true; } }
        object IList.this[int i] { get { return this.GetField(i); } set { this.SetField(i, value); } }
        bool IList.IsReadOnly { get { return false; } }
        int IList.Add(object value) { throw new NotSupportedException(); }
        void IList.Clear() { throw new NotSupportedException(); }
        bool IList.Contains(object value) { throw new NotSupportedException(); }
        int IList.IndexOf(object value) { throw new NotSupportedException(); }
        void IList.Insert(int index, object value) { throw new NotSupportedException(); }
        void IList.Remove(object value) { throw new NotSupportedException(); }
        void IList.RemoveAt(int index) { throw new NotSupportedException(); }
        #endregion
        #region ICollection only
        bool ICollection.IsSynchronized { get { return false; } }
        object ICollection.SyncRoot { get { return null; } }
        #endregion
        #region internal
        internal static Column CreateColumn(string name, TypeCode typeCode, RowCount rowCount)
        {
            if (typeCode == TypeCode.Empty)
            {
                throw new Exception("Empty: TypeCode not supported");
            }
            if (typeCode == TypeCode.DBNull)
            {
                throw new Exception("DBNull: TypeCode not supported");
            }
            if (typeCode == TypeCode.Object)
            {
                throw new Exception("Object: TypeCode not supported");
            }
            switch (typeCode)
            {
                case TypeCode.Boolean: return (Column)new Column<bool>(name, rowCount);
                case TypeCode.Byte: return (Column)new Column<byte>(name, rowCount);
                case TypeCode.Char: return (Column)new Column<char>(name, rowCount);
                case TypeCode.DateTime: return (Column)new Column<DateTime>(name, rowCount);
                case TypeCode.Decimal: return (Column)new Column<Decimal>(name, rowCount);
                case TypeCode.Double: return (Column)new Column<double>(name, rowCount);
                case TypeCode.Int16: return (Column)new Column<Int16>(name, rowCount);
                case TypeCode.Int32: return (Column)new Column<Int32>(name, rowCount);
                case TypeCode.Int64: return (Column)new Column<Int64>(name, rowCount);
                case TypeCode.SByte: return (Column)new Column<SByte>(name, rowCount);
                case TypeCode.Single: return (Column)new Column<Single>(name, rowCount);
                case TypeCode.String: return (Column)new Column<String>(name, rowCount);
                case TypeCode.UInt16: return (Column)new Column<UInt16>(name, rowCount);
                case TypeCode.UInt32: return (Column)new Column<UInt32>(name, rowCount);
                case TypeCode.UInt64: return (Column)new Column<UInt64>(name, rowCount);
            }
            return null;
        }
        internal Column(string name, TypeCode typeCode, RowCount rowCount)
        {
            _name = name;
            _typeCode = typeCode;
            _rowCount = rowCount;
        }
        internal void AddDefault()
        {
            AddDefaultType();
        }
        internal void SetDefault(int rowId)
        {
            SetDefaultType(rowId);
        }
        internal abstract void Truncate();
        internal abstract void RemoveRows(int index, int count = 1);
        #endregion
        #region protected
        protected abstract E Get<E>(int rowId);
        protected abstract object GetObject(int rowId);

        protected abstract void Set<E>(int rowId, E value);
        protected abstract void SetObject(int rowId, object value);
        protected abstract void AddDefaultType();
        protected abstract void SetDefaultType(int rowId);
        void ICollection.CopyTo(Array array, int index) { throw new Exception("Invalid operation on DynamicTable column"); }

        #endregion
        #region private
        private string _name;
        private TypeCode _typeCode;
        private RowCount _rowCount;
        #endregion
    }

    public class Column<T> : Column, IColumn<T>
    {
        public T this[int rowId]
        {
            get { return _list.GetItem<T>(rowId); }
            set { _list.SetItem(rowId, value); }
        }

        #region IEnumerable<T> only
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }
        #endregion
        #region IList<T> only
        int IList<T>.IndexOf(T item) { throw new Exception("Invalid operation on DynamicTable column");}
        void IList<T>.Insert(int index, T item) { throw new Exception("Invalid operation on DynamicTable column");}
        void IList<T>.RemoveAt(int index) { throw new Exception("Invalid operation on DynamicTable column");}
        #endregion
        #region ICollection only
        void ICollection<T>.Add(T item) { throw new Exception("Invalid operation on DynamicTable column");}
        void ICollection<T>.Clear() { throw new Exception("Invalid operation on DynamicTable column"); }
        bool ICollection<T>.Contains(T item) { throw new Exception("Invalid operation on DynamicTable column"); }
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) { throw new Exception("Invalid operation on DynamicTable column");}
        bool ICollection<T>.Remove(T item) { throw new Exception("Invalid operation on DynamicTable column"); }
        int ICollection<T>.Count { get { return this.Count; } }
        bool ICollection<T>.IsReadOnly { get { return ((IList)this).IsReadOnly; } }
        #endregion
        #region internal
        internal Column(string name, RowCount rowCount)
            : base(name, Type.GetTypeCode(typeof(T)), rowCount)
        {
            _list = DynamicList.createList(Type.GetTypeCode(typeof(T)));
            while (_list.Count<T>() < rowCount.Count) { _list.Add(default(T)); }
        }
        #endregion
        #region protected
        protected override E Get<E>(int rowId)
        {
            return _list.GetItem<E>(rowId);
        }
        protected override object GetObject(int rowId)
        {
            return _list.GetObject(TypeCode, rowId);
        }
        protected override void Set<E>(int rowId, E value)
        {
            _list.SetItem<E>(rowId, value);
        }
        protected override void SetObject(int rowId, object value)
        {
            _list.SetObject(TypeCode, rowId, value);
        }
        protected override void AddDefaultType()
        {
            _list.Add<T>(default(T));
        }
        protected override void SetDefaultType(int rowId)
        {
            _list.SetItem<T>(rowId, default(T));
        }
        internal override void Truncate()
        {
            _list.Clear<T>();
        }
        internal override void RemoveRows(int index, int count = 1)
        {
            _list.RemoveRange<T>(index, count);
        }
        #endregion
        #region private
        private DynamicList _list;
        #endregion
    }




}
