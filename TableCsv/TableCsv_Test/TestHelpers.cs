using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableCsv;

namespace TableCsv_Test
{
    internal static class TestHelpers
    {
        public static DynamicTable CreateSampleTable()
        {
            var table = new DynamicTable();
            table.columns.NewColumn("Id", TypeCode.Int32);
            table.columns.NewColumn("Name", TypeCode.String);
            table.columns.NewColumn("Amount", TypeCode.Double);

            AddRow(table, 1, "alpha", 10.5);
            AddRow(table, 2, "beta", 20.25);
            AddRow(table, 3, "gamma", 30.75);
            return table;
        }

        public static void AddRow(DynamicTable table, int id, string name, double amount)
        {
            var row = table.rows.NewRow();
            row.SetField("Id", id);
            row.SetField("Name", name);
            row.SetField("Amount", amount);
        }

        public static string WriteToString(IDynamicTable table, CsvHeaderType headerType)
        {
            using var writer = new StringWriter();
            CsvTable.Write(new CsvWriter(writer), table, headerType);
            return writer.ToString();
        }

        public static DynamicTable ReadFromString(string csv, CsvHeaderType headerType)
        {
            var table = new DynamicTable();
            using var reader = new StringReader(csv);
            CsvTable.Read(new CsvReader(reader), table, headerType, "");
            return table;
        }

        public static DynamicTable RoundTrip(IDynamicTable table, CsvHeaderType headerType)
        {
            return ReadFromString(WriteToString(table, headerType), headerType);
        }

        public static void AssertTablesEqual(IDynamicTable expected, IDynamicTable actual)
        {
            Assert.AreEqual(expected.columns.Count, actual.columns.Count);
            Assert.AreEqual(expected.rows.Count, actual.rows.Count);

            foreach (IColumn column in expected.columns)
            {
                Assert.IsTrue(actual.columns.ColumnExists(column.Name), $"Missing column '{column.Name}'.");
                var actualColumn = actual.columns[column.Name];
                Assert.AreEqual(column.TypeCode, actualColumn.TypeCode,
                    $"Type mismatch for column '{column.Name}'.");

                for (int row = 0; row < expected.rows.Count; row++)
                {
                    Assert.AreEqual(
                        column.GetFieldAsString(row),
                        actualColumn.GetFieldAsString(row),
                        $"Value mismatch at row {row}, column '{column.Name}'.");
                }
            }
        }
    }
}