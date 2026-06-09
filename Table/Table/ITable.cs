using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table
{
    public interface IColumn : IList
    {
        string Name { get; }
        TypeCode TypeCode { get; }
        T GetField<T>(int rowId);
        object GetField(int rowId);
        void SetField<T>(int rowId, T value);
        void SetField(int rowId, object obj);
        string GetFieldAsString(int rowId);
        void SetFieldAsString(int rowId, string value);
    }
    public interface IColumn<T> : IList<T>
    {
    }
    public interface IColumns : ICollection<IColumn>
    {
        IColumn this[string name] { get; }
        bool ColumnExists(string name);
        IColumn NewColumn(string name, TypeCode typeCode = TypeCode.String);
      }
    public interface IRow : ICollection
    {
        void UpdateFields(params KeyValuePair<string, object>[] values);
        void UpdateFields(params KeyValuePair<string, string>[] values);
        T GetField<T>(string name);
        string GetFieldAsString(string name);
        object GetFieldAsObject(string name);
        void SetField<T>(string name, T value);
        void SetFieldAsString(string name, string value);
        void SetFieldAsObject(string name, object value);
        IColumns columns { get; }
        int RowNo { get; }
    }
    public interface IRows : IList<IRow>
    {
        IRow NewRow();
        IColumns columns { get; }
        void RemoveRows(int index, int count = 1);
    }
    public interface IDynamicTable
    {
        IColumns columns { get; }
        IRows rows { get; }
    }
}
