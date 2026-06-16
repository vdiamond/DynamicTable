using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableExtension;

namespace TableExtension_Test
{
    [TestClass]
    public class TableCompositionTests
    {
        [TestMethod]
        public void SelectFrom_FiltersRowsByPredicate()
        {
            var table = TestHelpers.CreateMainTable();

            var filtered = table.SelectFrom(row => table.rows[row].GetField<int>("Id") > 1);

            Assert.AreEqual(2, filtered.rows.Count);
            Assert.AreEqual(2, filtered.rows[0].GetField<int>("Id"));
            Assert.AreEqual(3, filtered.rows[1].GetField<int>("Id"));
        }

        [TestMethod]
        public void CloneStructure_CopiesSchemaWithoutRows()
        {
            var table = TestHelpers.CreateMainTable();

            var clone = table.CloneStructure();

            Assert.AreEqual(2, clone.columns.Count);
            Assert.AreEqual(0, clone.rows.Count);
            Assert.AreEqual(TypeCode.Int32, clone.columns["Id"].TypeCode);
            Assert.AreEqual(TypeCode.String, clone.columns["Name"].TypeCode);
        }

        [TestMethod]
        public void Append_ConcatenatesIdenticalTables()
        {
            var first = new DynamicTable();
            first.columns.NewColumn("Id", TypeCode.Int32);
            first.rows.NewRow().SetField("Id", 1);

            var second = new DynamicTable();
            second.columns.NewColumn("Id", TypeCode.Int32);
            second.columns.NewColumn("Name", TypeCode.String);
            TestHelpers.AddRow(second, 2, "b");
            TestHelpers.AddRow(second, 3, "c");

            first.columns.NewColumn("Name", TypeCode.String);
            first.Append(second);

            Assert.AreEqual(3, first.rows.Count);
            Assert.AreEqual(1, first.rows[0].GetField<int>("Id"));
            Assert.AreEqual(2, first.rows[1].GetField<int>("Id"));
            Assert.AreEqual(3, first.rows[2].GetField<int>("Id"));
            Assert.AreEqual("b", first.rows[1].GetField<string>("Name"));
        }

        [TestMethod]
        public void AppendUnique_SkipsDuplicateKeys()
        {
            var target = new DynamicTable();
            target.columns.NewColumn("Id", TypeCode.Int32);
            target.columns.NewColumn("Name", TypeCode.String);
            TestHelpers.AddRow(target, 1, "existing");

            var source = new DynamicTable();
            source.columns.NewColumn("Id", TypeCode.Int32);
            source.columns.NewColumn("Name", TypeCode.String);
            TestHelpers.AddRow(source, 1, "duplicate");
            TestHelpers.AddRow(source, 2, "new");

            target.AppendUnique<int>(source, "Id");

            Assert.AreEqual(2, target.rows.Count);
            Assert.AreEqual("existing", target.rows[0].GetField<string>("Name"));
            Assert.AreEqual(2, target.rows[1].GetField<int>("Id"));
            Assert.AreEqual("new", target.rows[1].GetField<string>("Name"));
        }
    }
}