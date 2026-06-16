using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableExtension;

namespace TableExtension_Test
{
    [TestClass]
    public class SortTests
    {
        [TestMethod]
        public void Sort_Ascending_OrdersRowsByColumn()
        {
            var table = new DynamicTable();
            table.columns.NewColumn("Id", TypeCode.Int32);
            table.columns.NewColumn("Name", TypeCode.String);
            TestHelpers.AddRow(table, 30, "c");
            TestHelpers.AddRow(table, 10, "a");
            TestHelpers.AddRow(table, 20, "b");

            var sorted = table.Sort<int>(table.columns["Id"]);

            Assert.AreEqual(3, sorted.rows.Count);
            Assert.AreEqual(10, sorted.rows[0].GetField<int>("Id"));
            Assert.AreEqual("a", sorted.rows[0].GetField<string>("Name"));
            Assert.AreEqual(20, sorted.rows[1].GetField<int>("Id"));
            Assert.AreEqual(30, sorted.rows[2].GetField<int>("Id"));
        }

        [TestMethod]
        public void Sort_Descending_OrdersRowsByColumn()
        {
            var table = new DynamicTable();
            table.columns.NewColumn("Id", TypeCode.Int32);
            table.rows.NewRow().SetField("Id", 10);
            table.rows.NewRow().SetField("Id", 30);
            table.rows.NewRow().SetField("Id", 20);

            var sorted = table.Sort<int>(table.columns["Id"], descending: true);

            Assert.AreEqual(30, sorted.rows[0].GetField<int>("Id"));
            Assert.AreEqual(20, sorted.rows[1].GetField<int>("Id"));
            Assert.AreEqual(10, sorted.rows[2].GetField<int>("Id"));
        }
    }
}