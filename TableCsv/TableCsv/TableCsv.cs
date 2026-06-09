using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Table;

namespace TableCsv
{
    public class Field
    {
        public Field(string name, TypeCode typeCode, bool isKey = false)
        {
            _name = name;
            _typeCode = typeCode;
            _isKey = isKey;
        }
        public string Name { get { return _name; } }
        public TypeCode TypeCode { get { return _typeCode; } }
        public bool IsKey { get { return _isKey; } }
        #region private
        private string _name;
        private TypeCode _typeCode;
        private bool _isKey;
        #endregion
    }
    public class DefaultFieldAttributes
    {
        public DefaultFieldAttributes()
        {
            _fields = new Dictionary<string, Field>();
            _key = null;
            _default = null;
        }
        public void Reset() { _fields.Clear(); _key = null; _default = null; }
        public bool Add(Field field)
        {
            if (_fields.ContainsKey(field.Name)) { return false; }
            if (field.IsKey && _key != null) { return false; }
            _fields.Add(field.Name, field);
            if (field.IsKey) { _key = field; }
            return true;
        }
        public void SetDefault(TypeCode typeCode)
        {
            _default = new Field(null, typeCode);
        }
        public Field Key { get { return _key; } }
        public Field Default {  get { return _default; } }      
        public Field this[string name]
        {
            get
            {
                Field field = null;
                if (_fields.TryGetValue(name, out field))
                {
                    return field;
                }
                if (_default != null)
                {
                    return _default;
                }
                return null;
            }
        }
        #region private
        private Dictionary<string, Field> _fields;
        private Field _key;
        private Field _default;
        #endregion
    }
    public enum CsvHeaderType { None, Header, TypedHeader }
    public static class CsvTable
    {
        public static void Write(CsvWriter cfw, IDynamicTable table, CsvHeaderType headerType)
        {
            IColumn[] columns = new IColumn[table.columns.Count];
            int c = 0;
            foreach (IColumn col in table.columns) { columns[c] = col; c++; }
            List<string> fields = new List<string>();
            while (fields.Count < columns.Length) { fields.Add(null); }

            switch (headerType)
            {
                case CsvHeaderType.TypedHeader:
                    _emit_typed_headers(cfw, columns, fields);
                    break;
                case CsvHeaderType.Header:
                    _emit_headers(cfw, columns, fields);
                    break;
            }

            for (int r = 0; r < table.rows.Count; r++)
            {
                for (c = 0; c < table.columns.Count; c++)
                {
                    fields[c] = columns[c].GetFieldAsString(r);
                }
                cfw.WriteRow(fields);
            }
            cfw.Flush();

        }
        public static void Read(CsvReader cfr, IDynamicTable table, CsvHeaderType headerType, string prefix, DefaultFieldAttributes attr = null)
        {
            IColumn[] columns = null;
            var fields = new List<string>();
            bool res = false;

            if (!cfr.ReadRow(fields)) { throw new Exception("Error reading table - stream empty"); }
            columns = new IColumn[fields.Count];

            switch (headerType)
            {
                case CsvHeaderType.TypedHeader:
                    _create_typed_headers(table, columns, fields, prefix, attr);
                    res = cfr.ReadRow(fields);
                    break;
                case CsvHeaderType.Header:
                    _create_headers(table, columns, fields, prefix, attr);
                    res = cfr.ReadRow(fields);
                    break;
                default:
                    _create(table, columns, fields, prefix, attr);
                    break;
            }

            while (res)
            {
                IRow row = table.rows.NewRow();
                for (int c = 0; c < columns.Length; c++)
                {
                    row.SetFieldAsString(columns[c].Name, fields[c]);
                }
                res = cfr.ReadRow(fields);
            }
        }
        public static char HeaderTypeSeparator { get { return _headerTypeSeparator; } set { _headerTypeSeparator = value; } }
        #region Private
        private static void _create_typed_headers(IDynamicTable table, IColumn[] columns, List<string> fields, string prefix, DefaultFieldAttributes attr)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                string[] name_type = fields[i].Split(_headerTypeSeparator);
                Field field = null;
                if (attr != null) { field = attr[name_type[0]]; }
                bool key = false;
                if (field != null) { key = field.IsKey ; }
                name_type[0] = _prefixName(prefix, key, name_type[0]);
                var tc = _stringToType(name_type[1]);
                if (field != null) { tc = field.TypeCode; }
                columns[i] = table.columns.NewColumn(name_type[0], tc);
            }
        }
        private static void _create_headers(IDynamicTable table, IColumn[] columns, List<string> fields, string prefix, DefaultFieldAttributes attr)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                Field field = null;
                if (attr != null) { field = attr[fields[i]]; }
                bool key = false;
                if (field != null) { key = field.IsKey; }
                fields[i] = _prefixName(prefix, key, fields[i]);
                var tc = TypeCode.String;
                if (field != null) { tc = field.TypeCode; }
                columns[i] = table.columns.NewColumn(fields[i], tc);
            }
        }
        private static void _create(IDynamicTable table, IColumn[] columns, List<string> fields, string prefix, DefaultFieldAttributes attr)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                string name = string.Format("Col({0})", i);
                Field field = null;
                if (attr != null) { field = attr[fields[i]]; }
                bool key = false;
                if (field != null) { key = field.IsKey; }
                name = _prefixName(prefix, key, name);
                var tc = TypeCode.String;
                if (field != null) { tc = field.TypeCode; }
                columns[i] = table.columns.NewColumn(name, tc);
            }
        }
        private static string _prefixName(string prefix, bool key, string name)
        {
            if (prefix == null) return name;
            if (prefix == "") return name;
            if (key) { return name; }   
            return prefix + "." + name; 
        }
        private static TypeCode _stringToType(string t)
        {
            if ((TypeCode.Boolean).ToString() == t) return TypeCode.Boolean;
            if ((TypeCode.Byte).ToString() == t) return TypeCode.Byte;
            if ((TypeCode.Char).ToString() == t) return TypeCode.Char;
            if ((TypeCode.DateTime).ToString() == t) return TypeCode.DateTime;
            if ((TypeCode.Decimal).ToString() == t) return TypeCode.Decimal;
            if ((TypeCode.Double).ToString() == t) return TypeCode.Double;
            if ((TypeCode.Int16).ToString() == t) return TypeCode.Int16;
            if ((TypeCode.Int32).ToString() == t) return TypeCode.Int32;
            if ((TypeCode.Int64).ToString() == t) return TypeCode.Int64;
            if ((TypeCode.SByte).ToString() == t) return TypeCode.SByte;
            if ((TypeCode.Single).ToString() == t) return TypeCode.Single;
            if ((TypeCode.String).ToString() == t) return TypeCode.String;
            if ((TypeCode.UInt16).ToString() == t) return TypeCode.UInt16;
            if ((TypeCode.UInt32).ToString() == t) return TypeCode.UInt32;
            if ((TypeCode.UInt64).ToString() == t) return TypeCode.UInt64;
            throw new Exception("Invalid type on column (" + t + ")");
        }
        private static void _emit_typed_headers(CsvWriter cfw, IColumn[] columns, List<string> fields)
        {
            for (int c = 0; c < columns.Length; c++)
            {
                fields[c] = columns[c].Name + _headerTypeSeparator + columns[c].TypeCode.ToString();
            }
            cfw.WriteRow(fields);
        }
        private static void _emit_headers(CsvWriter cfw, IColumn[] columns, List<string> fields)
        {
            for (int c = 0; c < columns.Length; c++)
            {
                fields[c] = columns[c].Name;
            }
            cfw.WriteRow(fields);
        }
        private static char _headerTypeSeparator = '@';
        #endregion
    }
}
