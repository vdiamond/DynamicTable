using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;

namespace Table_Test
{
    [TestClass]
    public class TableLifecycleTests
    {
        [TestMethod]
        public void NewRow_InitializesAllColumnsWithDefaults()
        {
            var table = TestHelpers.CreateTable(
                ("BoolCol", TypeCode.Boolean),
                ("IntCol", TypeCode.Int32),
                ("StrCol", TypeCode.String));

            table.rows.NewRow();

            Assert.AreEqual(1, table.rows.Count);
            foreach (IColumn column in table.columns)
            {
                TestHelpers.AssertDefaultValue(column, 0);
            }
            TestHelpers.AssertColumnRowCountSync(table);
        }

        [TestMethod]
        public void NewColumn_AfterRowsExist_BackfillsDefaultsWithoutDisturbingExistingData()
        {
            var table = TestHelpers.CreateTable(("Existing", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Existing", 100));
            TestHelpers.AddRow(table, ("Existing", 200));
            TestHelpers.AddRow(table, ("Existing", 300));

            table.columns.NewColumn("Added", TypeCode.String);

            Assert.AreEqual(2, table.columns.Count);
            Assert.AreEqual(3, table.rows.Count);
            Assert.AreEqual(100, table.rows[0].GetField<int>("Existing"));
            Assert.AreEqual(200, table.rows[1].GetField<int>("Existing"));
            Assert.AreEqual(300, table.rows[2].GetField<int>("Existing"));
            Assert.IsNull(table.rows[0].GetField<string>("Added"));
            Assert.IsNull(table.rows[1].GetField<string>("Added"));
            Assert.IsNull(table.rows[2].GetField<string>("Added"));
            TestHelpers.AssertColumnRowCountSync(table);
        }

        [TestMethod]
        public void RemoveRows_MiddleSlice_PreservesRemainingValues()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32), ("Name", TypeCode.String));
            TestHelpers.AddRow(table, ("Id", 1), ("Name", "one"));
            TestHelpers.AddRow(table, ("Id", 2), ("Name", "two"));
            TestHelpers.AddRow(table, ("Id", 3), ("Name", "three"));
            TestHelpers.AddRow(table, ("Id", 4), ("Name", "four"));
            TestHelpers.AddRow(table, ("Id", 5), ("Name", "five"));

            table.rows.RemoveRows(1, 2);

            Assert.AreEqual(3, table.rows.Count);
            Assert.AreEqual(1, table.rows[0].GetField<int>("Id"));
            Assert.AreEqual("one", table.rows[0].GetField<string>("Name"));
            Assert.AreEqual(4, table.rows[1].GetField<int>("Id"));
            Assert.AreEqual("four", table.rows[1].GetField<string>("Name"));
            Assert.AreEqual(5, table.rows[2].GetField<int>("Id"));
            Assert.AreEqual("five", table.rows[2].GetField<string>("Name"));
            TestHelpers.AssertColumnRowCountSync(table);
        }

        [TestMethod]
        public void Clear_RemovesAllRowsButKeepsColumns()
        {
            var table = TestHelpers.CreateTable(("Col1", TypeCode.String), ("Col2", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Col1", "a"), ("Col2", 1));
            TestHelpers.AddRow(table, ("Col1", "b"), ("Col2", 2));

            table.rows.Clear();

            Assert.AreEqual(0, table.rows.Count);
            Assert.AreEqual(2, table.columns.Count);
            foreach (IColumn column in table.columns)
            {
                Assert.AreEqual(0, column.Count);
            }
            TestHelpers.AssertColumnRowCountSync(table);
        }

        [TestMethod]
        public void ColumnCount_AlwaysMatchesRowCount_AfterAddRemoveAndClear()
        {
            var table = TestHelpers.CreateTable(
                ("A", TypeCode.Int32),
                ("B", TypeCode.String),
                ("C", TypeCode.Double));

            TestHelpers.AddRow(table, ("A", 1), ("B", "x"), ("C", 1.1));
            TestHelpers.AddRow(table, ("A", 2), ("B", "y"), ("C", 2.2));
            TestHelpers.AssertColumnRowCountSync(table);

            table.rows.RemoveRows(0, 1);
            TestHelpers.AssertColumnRowCountSync(table);

            table.rows.NewRow();
            TestHelpers.AssertColumnRowCountSync(table);

            table.rows.Clear();
            TestHelpers.AssertColumnRowCountSync(table);
        }

        [TestMethod]
        public void SmokeTest_CreateTableWithRows_MatchesExpectedCounts()
        {
            var table = TestHelpers.CreateTable(("Col1", TypeCode.String), ("Col2", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Col1", "111"), ("Col2", 222));
            TestHelpers.AddRow(table, ("Col1", "333"), ("Col2", 444));
            TestHelpers.AddRow(table, ("Col1", "555"), ("Col2", 666));
            TestHelpers.AddRow(table, ("Col1", "777"), ("Col2", 888));

            Assert.AreEqual(2, table.columns.Count);
            Assert.AreEqual(4, table.rows.Count);
        }
    }
}