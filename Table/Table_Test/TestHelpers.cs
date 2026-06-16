using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;

namespace Table_Test
{
    internal static class TestHelpers
    {
        public static DynamicTable CreateTable(params (string name, TypeCode typeCode)[] columns)
        {
            var table = new DynamicTable();
            foreach (var (name, typeCode) in columns)
            {
                table.columns.NewColumn(name, typeCode);
            }
            return table;
        }

        public static void AddRow(DynamicTable table, params (string name, object value)[] fields)
        {
            var row = table.rows.NewRow();
            foreach (var (name, value) in fields)
            {
                row.SetFieldAsObject(name, value);
            }
        }

        public static void AssertColumnRowCountSync(IDynamicTable table)
        {
            foreach (IColumn column in table.columns)
            {
                Assert.AreEqual(table.rows.Count, column.Count,
                    $"Column '{column.Name}' count should match row count.");
            }
        }

        public static object SampleValue(TypeCode typeCode)
        {
            return typeCode switch
            {
                TypeCode.Boolean => true,
                TypeCode.Byte => (byte)42,
                TypeCode.Char => 'X',
                TypeCode.DateTime => new DateTime(2024, 6, 15, 10, 30, 0),
                TypeCode.Decimal => 123.45m,
                TypeCode.Double => 3.14159,
                TypeCode.Int16 => (short)-123,
                TypeCode.Int32 => 98765,
                TypeCode.Int64 => 9223372036854775807L,
                TypeCode.SByte => (sbyte)-7,
                TypeCode.Single => 2.5f,
                TypeCode.String => "sample",
                TypeCode.UInt16 => (ushort)65000,
                TypeCode.UInt32 => 4000000000u,
                TypeCode.UInt64 => 18446744073709551615UL,
                _ => throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, "Unsupported TypeCode")
            };
        }

        public static string SampleString(TypeCode typeCode)
        {
            return typeCode switch
            {
                TypeCode.Boolean => "true",
                TypeCode.Double => "3.14159",
                TypeCode.Int32 => "98765",
                TypeCode.DateTime => "2024-06-15T10:30:00",
                TypeCode.String => "sample",
                _ => SampleValue(typeCode).ToString()!
            };
        }

        public static void AssertDefaultValue(IColumn column, int rowId)
        {
            switch (column.TypeCode)
            {
                case TypeCode.Boolean:
                    Assert.AreEqual(false, column.GetField<bool>(rowId));
                    break;
                case TypeCode.Byte:
                    Assert.AreEqual((byte)0, column.GetField<byte>(rowId));
                    break;
                case TypeCode.Char:
                    Assert.AreEqual('\0', column.GetField<char>(rowId));
                    break;
                case TypeCode.DateTime:
                    Assert.AreEqual(default(DateTime), column.GetField<DateTime>(rowId));
                    break;
                case TypeCode.Decimal:
                    Assert.AreEqual(0m, column.GetField<decimal>(rowId));
                    break;
                case TypeCode.Double:
                    Assert.AreEqual(0d, column.GetField<double>(rowId));
                    break;
                case TypeCode.Int16:
                    Assert.AreEqual((short)0, column.GetField<short>(rowId));
                    break;
                case TypeCode.Int32:
                    Assert.AreEqual(0, column.GetField<int>(rowId));
                    break;
                case TypeCode.Int64:
                    Assert.AreEqual(0L, column.GetField<long>(rowId));
                    break;
                case TypeCode.SByte:
                    Assert.AreEqual((sbyte)0, column.GetField<sbyte>(rowId));
                    break;
                case TypeCode.Single:
                    Assert.AreEqual(0f, column.GetField<float>(rowId));
                    break;
                case TypeCode.String:
                    Assert.IsNull(column.GetField<string>(rowId));
                    break;
                case TypeCode.UInt16:
                    Assert.AreEqual((ushort)0, column.GetField<ushort>(rowId));
                    break;
                case TypeCode.UInt32:
                    Assert.AreEqual(0u, column.GetField<uint>(rowId));
                    break;
                case TypeCode.UInt64:
                    Assert.AreEqual(0UL, column.GetField<ulong>(rowId));
                    break;
                default:
                    Assert.Fail($"Unexpected TypeCode: {column.TypeCode}");
                    break;
            }
        }
    }
}