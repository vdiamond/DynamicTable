using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;

namespace Table_Test
{
    [TestClass]
    public class RowTests
    {
        [TestMethod]
        public void RowIndexer_ReturnsCorrectRowNoAndValues()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32), ("Label", TypeCode.String));
            TestHelpers.AddRow(table, ("Id", 42), ("Label", "answer"));
            TestHelpers.AddRow(table, ("Id", 7), ("Label", "lucky"));

            Assert.AreEqual(0, table.rows[0].RowNo);
            Assert.AreEqual(42, table.rows[0].GetField<int>("Id"));
            Assert.AreEqual("answer", table.rows[0].GetField<string>("Label"));

            Assert.AreEqual(1, table.rows[1].RowNo);
            Assert.AreEqual(7, table.rows[1].GetField<int>("Id"));
            Assert.AreEqual("lucky", table.rows[1].GetField<string>("Label"));
        }

        [TestMethod]
        public void UpdateFields_StringOverload_SetsValues()
        {
            var table = TestHelpers.CreateTable(("Name", TypeCode.String), ("Count", TypeCode.Int32));
            var row = table.rows.NewRow();

            row.UpdateFields(new[]
            {
                new KeyValuePair<string, string>("Name", "gamma"),
                new KeyValuePair<string, string>("Count", "99")
            });

            Assert.AreEqual("gamma", row.GetField<string>("Name"));
            Assert.AreEqual(99, row.GetField<int>("Count"));
        }

        [TestMethod]
        public void RowEnumeration_YieldsValuesInColumnOrder()
        {
            var table = TestHelpers.CreateTable(("First", TypeCode.Int32), ("Second", TypeCode.String));
            TestHelpers.AddRow(table, ("First", 1), ("Second", "one"));

            var values = new List<object>();
            foreach (var value in table.rows[0])
            {
                values.Add(value);
            }

            CollectionAssert.AreEqual(new object[] { 1, "one" }, values);
        }

        [TestMethod]
        public void RemoveAt_RemovesSingleRow()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Id", 1));
            TestHelpers.AddRow(table, ("Id", 2));
            TestHelpers.AddRow(table, ("Id", 3));

            table.rows.RemoveAt(1);

            Assert.AreEqual(2, table.rows.Count);
            Assert.AreEqual(1, table.rows[0].GetField<int>("Id"));
            Assert.AreEqual(3, table.rows[1].GetField<int>("Id"));
            TestHelpers.AssertColumnRowCountSync(table);
        }

        [TestMethod]
        public void Remove_ByRowReference_RemovesSingleRow()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Id", 10));
            TestHelpers.AddRow(table, ("Id", 20));
            var rowToRemove = table.rows[0];

            Assert.IsTrue(table.rows.Remove(rowToRemove));

            Assert.AreEqual(1, table.rows.Count);
            Assert.AreEqual(20, table.rows[0].GetField<int>("Id"));
            TestHelpers.AssertColumnRowCountSync(table);
        }

        [TestMethod]
        public void Contains_ReturnsTrueForValidRowAndFalseForOutOfRangeRowNo()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Id", 1));

            Assert.IsTrue(table.rows.Contains(table.rows[0]));

            var staleRow = table.rows[0];
            table.rows.RemoveAt(0);
            Assert.IsFalse(table.rows.Contains(staleRow));
        }

        [TestMethod]
        public void IndexOf_ReturnsRowNumber()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Id", 1));
            TestHelpers.AddRow(table, ("Id", 2));

            Assert.AreEqual(1, table.rows.IndexOf(table.rows[1]));
        }
    }
}