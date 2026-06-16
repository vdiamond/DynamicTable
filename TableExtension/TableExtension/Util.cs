using Table;
using TableCsv;
using System.Numerics;
using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Net;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace TableExtension
{
    public class ColumnTransform
    {
        public ColumnTransform(string sourceName, bool prefix = false) 
        { 
            _sourceName = sourceName;
            _destName = sourceName;
            _rename = false;
            _prefix = prefix;
        }
        public ColumnTransform(string sourceName, string destName, bool prefix = false)
        {
            _sourceName = sourceName;
            _destName = destName;
            _rename = true;
            _prefix = prefix;
        }
        public string SourceName() { return _sourceName; }    
        public string DestName(string prefix = null) 
        {
            string ret = _destName;
            if (_prefix && prefix != null)
            {
                ret = $"{prefix}.{ret}";
            }
            return ret; 
        }
        #region private
        private string _sourceName;
        private string _destName;
        private bool _rename;
        private bool _prefix;
        #endregion
    }
    public static class UtilExtensions
    {
        //
        //
        // Extensions for IDynamicTable
        public static void LoadCsv(this IDynamicTable dt, TextReader reader, CsvHeaderType cht, string prefix = null, DefaultFieldAttributes attr = null)
        {
            dt.columns.Clear();
            var cfr = new CsvReader(reader);
            CsvTable.Read(cfr, dt, cht, prefix, attr);
        }
        public static void WriteCsv(this IDynamicTable dt, TextWriter writer, CsvHeaderType cht)
        {
            var cfw = new CsvWriter(writer);
            CsvTable.Write(cfw, dt, cht);
        }
        public static void LoadCsv(this IDynamicTable dt, string path, CsvHeaderType cht, string prefix = null, DefaultFieldAttributes attr = null)
        {
            using var reader = new StreamReader(path);  
            dt.columns.Clear();
            var cfr = new CsvReader(reader);
            CsvTable.Read(cfr, dt, cht, prefix, attr);
        }
        public static void WriteCsv(this IDynamicTable dt, string path, CsvHeaderType cht)
        {
            using var writer = new StreamWriter(path);
            var cfw = new CsvWriter(writer);
            CsvTable.Write(cfw, dt, cht);
        }
        public static IDynamicTable CloneStructure(this IDynamicTable dt) 
        { 
            var ret = new DynamicTable();
            foreach ( var c in dt.columns) 
            {
                ret.columns.NewColumn(c.Name, c.TypeCode);
            }
            return ret;
        }
        public static IDynamicTable Sort<T>(this IDynamicTable idt, IColumn column, bool descending = false) 
        {
            var ret = idt.CloneStructure();
            var space_key = new T[column.Count];
            var col = (IColumn<T>)column;
            for (int i = 0; i < column.Count; i++)
            {
                space_key[i] = col[i];
            }
            Span<T> keys = space_key.AsSpan<T>();

            var space_value = new int[column.Count];
            Span<int> values = space_value.AsSpan<int>();
            for (int i = 0; i < values.Length; i++) { values[i] = i; }

            keys.Sort<T, int>(values);

            // Now we want to unpack into the new table
            if (!descending)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    ret.rows.NewCopiedFrom(idt.rows[values[i]]);
                }
            }
            else
            {
                int j = keys.Length - 1;
                for (int i = 0; i < keys.Length; i++)
                {
                    ret.rows.NewCopiedFrom(idt.rows[values[j]]);
                    j--;
                }
            }
            return ret;
        }
        public static IDynamicTable LeftJoin<K, V>(this IDynamicTable main, IDictionary<K, int> destKey, IColumn<K> sourceKey, string prefix, params IColumn<V>[] sourceCols)
        {
            // check to see that we have at least one column to add
            if (sourceCols.Length == 0) { throw new Exception("Need to have at least one column to add"); }

            // ensure that all the columns to be added have the same length as the source key
            foreach (var cc in sourceCols) { if (cc.Count != sourceKey.Count) { throw new Exception($"{((IColumn) cc).Name} count ({cc.Count}) does not match count of key ({sourceKey.Count})"); } }

            // add columns to the main table
            var cols = new IColumn<V>[sourceCols.Length];
            for (int i = 0; i < cols.Length;  i++)
            {
                var newColumnName = ((IColumn)sourceCols[i]).Name;
                if (prefix != null) { newColumnName = prefix + "." + newColumnName; }
                cols[i] = (IColumn<V>) main.columns.NewColumn(newColumnName, Type.GetTypeCode(typeof(V)));
            }

            // loop through source columns
            for (int i = 0; i < sourceKey.Count; i++)
            {
                if (!destKey.ContainsKey(sourceKey[i])) {  continue; }
                int place = destKey[sourceKey[i]];

                for (int j = 0; j < sourceCols.Length; j++)
                {
                    cols[j][place] = sourceCols[j][i];
                }

            }

            // return table
            return main;
        }
        public static IDictionary<T, int> BuildKey<T>(this IDynamicTable idt, IColumn<T> key)
        {
            var ret = new Dictionary<T, int>();
            var rows = idt.rows;

            for (int i = 0; i < rows.Count; i++)
            {
                ret.Add(key[i], i);
            }
            return ret;
        }
        public static IDynamicTable ColumnSubset(this IDynamicTable source, string prefix = null, params ColumnTransform[] destTransform)
        {
            if (destTransform.Length == 0) { throw new Exception("Cannot transform to zero columns"); }
            var sourceColumns = new Column[destTransform.Length];
            var destColumns = new Column[destTransform.Length];
            var ret = new DynamicTable();
            for (int i = 0; i < destTransform.Length; i++)
            {
                sourceColumns[i] = (Column) source.columns[destTransform[i].SourceName()];
                ret.columns.NewColumn(destTransform[i].DestName(prefix), sourceColumns[i].TypeCode);
                destColumns[i] = (Column) ret.columns[destTransform[i].DestName(prefix)];
            }

            for (int r = 0; r < source.rows.Count; r++)
            {
                var sourceRow = source.rows[r];
                var destRow = ret.rows.NewRow();
                for (int c = 0; c < destColumns.Count(); c++)
                {
                    destColumns[c].SetField(r, sourceColumns[c].GetField(r));
                }
            }
            return ret;
        }
        public static IList<T> AsList<T>(this IDynamicTable source, string name)
        {
            return (IList<T>) source.columns[name];
        }
        public static DataTable ToDataTable(this IDynamicTable source)
        {
            var dataTable = new DataTable();
            foreach (var col in source.columns)
            {
                dataTable.Columns.Add(new DataColumn(col.Name, GetTypeFromTypeCode(col.TypeCode)));
            }
            dataTable.BeginLoadData();
            for (int i = 0; i < source.rows.Count; i++)
            {
                var r = source.rows[i];
                var nr = dataTable.NewRow();
                foreach(var c in r.columns)
                {
                    nr[c.Name] = r.GetFieldAsObject(c.Name);
                }
                dataTable.Rows.Add(nr);
             }
            dataTable.EndLoadData();
            return dataTable;
        }
        public static IDynamicTable ToTable(this DataTable source)
        {
            var table = new DynamicTable();
            foreach (DataColumn col in source.Columns)
            {
                table.columns.NewColumn(col.ColumnName, Type.GetTypeCode(col.DataType));
            }

            for (int i = 0; i < source.Rows.Count; i++)
            {
                var r = source.Rows[i];
                var nr = table.rows.NewRow();   
                foreach(var c in nr.columns)
                {
                    nr.SetFieldAsObject(c.Name, r[c.Name]);
                }
            }
            return table;
        }
        public static IDynamicTable? GetStructure<T>(this T source)
        {
            Type tp = source.GetType();
            if (Type.GetTypeCode(tp) != TypeCode.Object) { return null; }
            PropertyInfo[] properties = tp.GetProperties();
            IDynamicTable? table = null;
            foreach (PropertyInfo property in properties)
            {
                var ptc = Type.GetTypeCode(property.PropertyType);
                if (ptc == TypeCode.DBNull || ptc == TypeCode.Empty || ptc == TypeCode.Object) { continue; }
                if (table == null) { table = new DynamicTable(); }
                table.columns.NewColumn(property.Name, ptc);
            }
            return table;
        }
        public static IList<T> ToClass<T>(this IDynamicTable table, IList<T>? ret = null)
        {
            // Load columns into a table, populate cross reference to property and validate / name & type
            Type t = typeof(T);
            PropertyInfo[] p = t.GetProperties();
            var plist = new List<PropertyInfo>();
            var pcol = new List<IColumn>();
            foreach (var col in table.columns)
            {
                // find it in PropertyInfo
                for (int i = 0; i < p.Length; i++)
                {
                    if (p[i].Name == col.Name)
                    {
                        if (Type.GetTypeCode(p[i].PropertyType) == col.TypeCode)
                        {
                            plist.Add(p[i]);
                            pcol.Add(col);
                        }
                    }
                }
            }

            // if there are no elements, just return
            if (plist.Count == 0) { return ret; }

            // if ret is null, create a list.
            ret = new List<T>();

            // Load each row of the dynamic table
            for (int i = 0; i < table.rows.Count; i++)
            {
                var e = Activator.CreateInstance(t);
                for (int j = 0; j < plist.Count; j++)
                {
                    plist[j].SetValue(e, pcol[j].GetField(i));
                }
                ret.Add((T)e);
            }

            // return it
            return ret;
        }
        public static IDynamicTable ToTable<T>(this IEnumerable<T> list, IDynamicTable? ret = null)
        {
            // Load columns into a table, populate cross reference to property and validate / name & type
            Type t = typeof(T);
            PropertyInfo[] p = t.GetProperties();
            var plist = new List<PropertyInfo>();
            var pcol = new List<IColumn>();
            if (ret == null)
            {
                var e = (T) Activator.CreateInstance(t);
                ret = e.GetStructure<T>();
            }
            foreach (var col in ret.columns)
            {
                // find it in PropertyInfo
                for (int i = 0; i < p.Length; i++)
                {
                    if (p[i].Name == col.Name)
                    {
                        if (Type.GetTypeCode(p[i].PropertyType) == col.TypeCode)
                        {
                            plist.Add(p[i]);
                            pcol.Add(col);
                        }
                    }
                }
            }

            // if there are no elements, just return
            if (plist.Count == 0) { return ret; }

            // Load each row of the dynamic table
            foreach (var e in list)
            {
                var r = ret.rows.NewRow();
                int ridx = ret.rows.Count - 1;
                for (int j = 0; j < plist.Count; j++)
                {
                    pcol[j].SetField(ridx, plist[j].GetValue(e));
                }
            }

            // return it
            return ret;
        }

        // old ==================================================================
        public static void RefreshFrom<T>(this IDynamicTable idt, IColumn toKey, IDynamicTable source, IColumn fromKey, bool allowNewColumns = true) where T : IComparable<T>
        {
            // First check if we need to add new columns, if we do, then add them
            if (allowNewColumns)
            {
                idt.columns.IncludeColumnDefinitions(source.columns);
            }

            // create mapping
            var temp = new int[fromKey.Count];
            var mapping = temp.AsSpan<int>();
            ((IColumn<T>) toKey).CreateMapInto<T>((IColumn<T>) fromKey, mapping);

            // do the copy
            idt.MapInto(mapping, source);
        }
        public static void MapInto(this IDynamicTable target, Span<int> mapping, IDynamicTable toMap)
        {
            if (toMap.rows.Count != mapping.Length) { throw new Exception("mapping length != toMap length"); }
            for (int i = 0; i < mapping.Length; i++) 
            {
                if (mapping[i] > -1)
                {
                    target.rows[mapping[i]].CopyFrom(toMap.rows[i]);
                }
            }
        }
        public static void MapInto(this IDynamicTable target, Span<int> mapping, params IColumn[] toMap) 
        {
            for (int i = 0; i < toMap.Length; i++)
            {
                if (target.columns.ColumnExists(toMap[i].Name))
                {
                    var tgt = target.columns[toMap[i].Name];
                    if (tgt.TypeCode == toMap[i].TypeCode)
                    {
                        for (int j = 0; j > mapping.Length; j++)
                        {
                            if (mapping[j] > -1)
                            {
                                tgt.SetField(mapping[j], toMap[i].GetField(j));
                            }
                        }
                    }
                }
            }
        }
        public static IDynamicTable GetSubTable(this IDynamicTable target, int start = 0, int length = 0)
        {
            var ret = target.CloneStructure();
            if (start >= target.rows.Count || start < 0) { throw new Exception("Start must be within table row range"); }
            if (length > 0) {  if (start + length > target.rows.Count) { throw new Exception("Length out of range of row range"); } }
            else { length = target.rows.Count - start; }
            int end = start + length;
            for (int r = start; r < end; r++)
            {
                ret.rows.NewCopiedFrom(target.rows[r]);
            }
            return ret;
        }
        public static void Append(this IDynamicTable target, IDynamicTable source, bool identicalStructure = true)
        {
            if (!target.columns.IsStructureEqual(source.columns) && identicalStructure) { throw new Exception("Cannot append table that does not have identical structure - set 'identicalStructure = false'"); }   
            if (!identicalStructure)
            {
                // check that at least one column is structurally in common
                if (target.columns.ColumnsInCommon(source.columns) == 0) 
                {
                    throw new Exception("Cannot append table that has no columns in common");
                } 
            }
            foreach (var rowToAdd in source.rows)
            {
                target.rows.NewCopiedFrom(rowToAdd);
            }
        }
        public static void AppendUnique<T>(this IDynamicTable target, IDynamicTable source, string key)
        {
            if (!target.columns.IsStructureEqual(source.columns)) { throw new Exception("Cannot append table that does not have identical structure"); }
            if (!target.columns.ColumnExists(key)) { throw new Exception(string.Format("Append key ({0}) does not exist", key)); }
            if (typeof(T) == typeof(IDynamicTable)) { throw new Exception("Mismatch in format of key"); }

            var dict = new Dictionary<T, int>();
            var keyCol = target.columns[key] as IColumn<T>;
            for (int i = 0; i < keyCol.Count; i++) 
            { 
                if (dict.ContainsKey(keyCol[i])) { throw new Exception(string.Format("Duplicates exist in base table - row {0} - is a duplicate", i)); }
                dict.Add(keyCol[i], i);
            }


            foreach (var rowToAdd in source.rows)
            {
                if (dict.ContainsKey(rowToAdd.GetField<T>(key))) { continue; }
                target.rows.NewCopiedFrom(rowToAdd);
                dict.Add(rowToAdd.GetField<T>(key), keyCol.Count);
            }
        }
        public static IDynamicTable SelectFrom(this IDynamicTable target, Func<int, bool> predicate) 
        {
            var ret = target.CloneStructure();
            for (int r = 0; r < target.rows.Count; r++)
            {
                if (predicate(r))
                {
                    ret.rows.NewCopiedFrom(target.rows[r]);
                }
            }
            return ret;
        }
        public static IDynamicTable MetaTable(this IDynamicTable target)
        {
            var idt = new DynamicTable();
            idt.columns.NewColumn("FieldName");
            idt.columns.NewColumn("FieldType");

            foreach (var col in target.columns)
            {
                var r = idt.rows.NewRow();
                r.SetField<string>("FieldName", col.Name);
                r.SetField<string>("FieldType", col.TypeCode.ToString());
            }

            return idt;
        }
        //
        //
        // Extensions for IRow
        public static void CopyFrom(this IRow target, IRow fromRow)
        {
            foreach (IColumn fromCol in fromRow.columns)
            {
                if (target.columns.ColumnExists(fromCol.Name))
                {
                    IColumn toCol = target.columns[fromCol.Name];
                    if (toCol.TypeCode == fromCol.TypeCode)
                    {
                        target.SetFieldAsObject(toCol.Name, fromRow.GetFieldAsObject(fromCol.Name));
                    }
                }
            }
        }
        //
        //
        // Extensions for IRows
        public static IRow NewCopiedFrom(this IRows rows, IRow rowToCopy)
        {
            IRow newRow = rows.NewRow();
            foreach (IColumn col in rows.columns)
            {
                if (!rowToCopy.columns.ColumnExists(col.Name)) {  continue; }
                if (rowToCopy.columns[col.Name].TypeCode != col.TypeCode) { continue; }
                newRow.SetFieldAsObject(col.Name, rowToCopy.GetFieldAsObject(col.Name));
            }
            return newRow;
        }
        //
        //
        // Extensions for IColumn
        public static void CopyToSpan<T>(this IColumn<T> target, Span<T> dest)
        {
            if (target.Count != dest.Length)
            {
                throw new Exception("Length mismatch with array in get statement");
            }
            for (int i = 0; i < target.Count; i++) { dest[i] = target[i]; }
        }
        public static void CopyFromSpan<T>(this IColumn<T> target, Span<T> source)
        {
            if (target.Count != source.Length)
            {
                throw new Exception("Length mismatch with array in get statement");
            }
            for (int i = 0; i < target.Count; i++) { target[i] = source[i]; }
        }
        public static void CreateMapInto<T>(this IColumn<T> target, IColumn<T> source, Span<int> mapping) where T : IComparable<T>
        {
            if (source.Count != mapping.Length) { throw new Exception("source length != mapping length"); }

            int tgt = 0;
            int comp = 0;
            for (int src = 0; src < source.Count; src++)
            {
                mapping[src] = -1;
                while (tgt < target.Count)
                {
                    comp = target[tgt].CompareTo(source[src]);
                    if (comp >= 0) { break; }
                    tgt++;
                }
                if (comp == 0) { mapping[src] = tgt; }
            }
        }
        //
        //
        // Extensions for IColumns
        public static IColumn<T> NewColumn<T>(this IColumns target, string name)
        {
           return (IColumn<T>)target.NewColumn(name, Type.GetTypeCode(typeof(T)));
        }
        public static IList<T> NewColumAsList<T>(this IColumns target, string name)
        {
            return (IList<T>)target.NewColumn(name, Type.GetTypeCode(typeof(T)));
        }

        public static void IncludeColumnDefinitions(this IColumns target, IColumns toInclude)
        {
            foreach (IColumn i in toInclude)
            {
                if (!target.ColumnExists(i.Name))
                {
                    target.NewColumn(i.Name, i.TypeCode);
                }
            }
        }
        public static void TrimToInclude(this IColumns target, params string[] columnNames)
        {
            foreach (var c in target)
            {
                if (!columnNames.Contains<string>(c.Name))
                {
                    target.Remove(c);
                }
            }
        }
        public static bool IsStructureEqual(this IColumns target, IColumns comparedTo)
        {
            if (target.Count != comparedTo.Count) { return false; }
            foreach(var col in comparedTo)
            {
                if (!target.ColumnExists(col.Name)) { return false; }
                if (col.TypeCode != target[col.Name].TypeCode) { return false; }    
            }
            return true;
        }
        public static int ColumnsInCommon(this IColumns target, IColumns comparedTo)
        {
            int ret = 0;
            foreach (var col in comparedTo)
            {
                if (target.ColumnExists(col.Name))
                {
                    if (col.TypeCode == target[col.Name].TypeCode) { ret++; }
                }
            }
            return ret;
        }
        //
        //
        // Private support routines
        #region private
        private static Type GetTypeFromTypeCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return typeof(bool);
                case TypeCode.Byte:
                    return typeof(byte);
                case TypeCode.Char:
                    return typeof(char);
                case TypeCode.DateTime:
                    return typeof(DateTime);
                case TypeCode.DBNull:
                    return typeof(DBNull);
                case TypeCode.Decimal:
                    return typeof(decimal);
                case TypeCode.Double:
                    return typeof(double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return typeof(short);
                case TypeCode.Int32:
                    return typeof(int);
                case TypeCode.Int64:
                    return typeof(long);
                case TypeCode.Object:
                    return typeof(object);
                case TypeCode.SByte:
                    return typeof(sbyte);
                case TypeCode.Single:
                    return typeof(float);
                case TypeCode.String:
                    return typeof(string);
                case TypeCode.UInt16:
                    return typeof(ushort);
                case TypeCode.UInt32:
                    return typeof(uint);
                case TypeCode.UInt64:
                    return typeof(ulong);
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }
        #endregion
    }

}