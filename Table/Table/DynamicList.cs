using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Table
{
    internal class DynamicList
    {
        #region internal
        static internal DynamicList createList(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: { return new DynamicList(new List<Boolean>()); }
                case TypeCode.Byte: { return new DynamicList(new List<Byte>()); }
                case TypeCode.Char: { return new DynamicList(new List<Char>()); }
                case TypeCode.DateTime: { return new DynamicList(new List<DateTime>()); }
                case TypeCode.Decimal: { return new DynamicList(new List<Decimal>()); }
                case TypeCode.Double: { return new DynamicList(new List<Double>()); }
                case TypeCode.Int16: { return new DynamicList(new List<Int16>()); }
                case TypeCode.Int32: { return new DynamicList(new List<Int32>()); }
                case TypeCode.Int64: { return new DynamicList(new List<Int64>()); }
                case TypeCode.SByte: { return new DynamicList(new List<SByte>()); }
                case TypeCode.Single: { return new DynamicList(new List<Single>()); }
                case TypeCode.String: { return new DynamicList(new List<String>()); }
                case TypeCode.UInt16: { return new DynamicList(new List<UInt16>()); }
                case TypeCode.UInt32: { return new DynamicList(new List<UInt32>()); }
                case TypeCode.UInt64: { return new DynamicList(new List<UInt64>()); }
            }
            return null;
        }
        internal void Add<T>(T val)
        {
            ((List<T>)_list).Add(val);
        }
        internal void AddEmpty<T>()
        {
            ((List<T>)_list).Add(default(T));
        }
        internal T GetItem<T>(int index)
        {
            return ((List<T>)_list)[index];
        }
        internal object GetObject(TypeCode typeCode, int index)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: { return ((List<Boolean>) _list)[index]; }
                case TypeCode.Byte: { return ((List<Byte>) _list)[index]; }
                case TypeCode.Char: { return ((List<Char>)_list)[index]; ; }
                case TypeCode.DateTime: { return ((List<DateTime>)_list)[index]; ; }
                case TypeCode.Decimal: { return ((List<Decimal>)_list)[index]; ; }
                case TypeCode.Double: { return ((List<Double>)_list)[index]; ; }
                case TypeCode.Int16: { return ((List<Int16>)_list)[index]; ; }
                case TypeCode.Int32: { return ((List<Int32>)_list)[index]; ; }
                case TypeCode.Int64: { return ((List<Int64>)_list)[index]; ; }
                case TypeCode.SByte: { return ((List<SByte>)_list)[index]; ; }
                case TypeCode.Single: { return ((List<Single>)_list)[index]; ; }
                case TypeCode.String: { return ((List<String>)_list)[index]; ; }
                case TypeCode.UInt16: { return ((List<UInt16>)_list)[index]; ; }
                case TypeCode.UInt32: { return ((List<UInt32>)_list)[index]; ; }
                case TypeCode.UInt64: { return ((List<UInt64>)_list)[index]; ; }
            }
            return null;
        }
        internal void SetItem<T>(int index, T item)
        {
            ((List<T>)_list)[index] = item;
        }
        internal void SetObject(TypeCode typeCode, int index, object item)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: { ((List<Boolean>)_list)[index] = (Boolean) item; return; }
                case TypeCode.Byte: { ((List<Byte>)_list)[index] = (Byte)item; return; }
                case TypeCode.Char: { ((List<Char>)_list)[index] = (Char) item; return; }
                case TypeCode.DateTime: { ((List<DateTime>)_list)[index] = (DateTime) item; return; }
                case TypeCode.Decimal: { ((List<Decimal>)_list)[index] = (Decimal) item; return; }
                case TypeCode.Double: { ((List<Double>)_list)[index] = (Double) item; return; }
                case TypeCode.Int16: { ((List<Int16>)_list)[index] = (Int16) item; return; }
                case TypeCode.Int32: { ((List<Int32>)_list)[index] = (Int32) item; return; }
                case TypeCode.Int64: { ((List<Int64>)_list)[index] = (Int64) item; return; }
                case TypeCode.SByte: { ((List<SByte>)_list)[index] = (SByte) item; return; }
                case TypeCode.Single: { ((List<Single>)_list)[index] = (Single) item; return; }
                case TypeCode.String: { ((List<String>)_list)[index] = (String) item; return; }
                case TypeCode.UInt16: { ((List<UInt16>)_list)[index] = (UInt16) item; return; }
                case TypeCode.UInt32: { ((List<UInt32>)_list)[index] = (UInt32) item; return; }
                case TypeCode.UInt64: { ((List<UInt64>)_list)[index] = (UInt64) item; return; }
            }   
        }
        internal int Count<T>()
        {
            return ((List<T>)_list).Count;
        }
        internal void Clear<T>()
        {
            ((List<T>)_list).Clear();
        }
        internal void RemoveRange<T>(int index, int count)
        {
            ((List<T>)_list).RemoveRange(index, count);
        }
        #endregion
        #region private
        private DynamicList(object list)
        {
            _list = list;
        }
        private object _list;
        #endregion
    }
}
