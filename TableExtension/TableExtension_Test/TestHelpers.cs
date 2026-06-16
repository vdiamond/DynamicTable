using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;

namespace TableExtension_Test
{
    internal static class TestHelpers
    {
        public static DynamicTable CreateMainTable()
        {
            var table = new DynamicTable();
            table.columns.NewColumn("Id", TypeCode.Int32);
            table.columns.NewColumn("Name", TypeCode.String);

            AddRow(table, 1, "alpha");
            AddRow(table, 2, "beta");
            AddRow(table, 3, "gamma");
            return table;
        }

        public static DynamicTable CreateLookupTable()
        {
            var table = new DynamicTable();
            table.columns.NewColumn("Id", TypeCode.Int32);
            table.columns.NewColumn("Score", TypeCode.Int32);

            AddLookupRow(table, 1, 100);
            AddLookupRow(table, 2, 200);
            return table;
        }

        public static void AddRow(DynamicTable table, int id, string name)
        {
            var row = table.rows.NewRow();
            row.SetField("Id", id);
            row.SetField("Name", name);
        }

        public static void AddLookupRow(DynamicTable table, int id, int score)
        {
            var row = table.rows.NewRow();
            row.SetField("Id", id);
            row.SetField("Score", score);
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