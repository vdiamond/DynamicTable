using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableCsv;

namespace TableCsv_Test
{
    [TestClass]
    public class CsvTableTests
    {
        [TestMethod]
        public void TypedHeader_RoundTrip_PreservesValues()
        {
            var original = TestHelpers.CreateSampleTable();

            var restored = TestHelpers.RoundTrip(original, CsvHeaderType.TypedHeader);

            Assert.AreEqual(3, restored.columns.Count);
            Assert.AreEqual(3, restored.rows.Count);
            Assert.AreEqual(TypeCode.Int32, restored.columns["Id"].TypeCode);
            Assert.AreEqual(TypeCode.String, restored.columns["Name"].TypeCode);
            Assert.AreEqual(TypeCode.Double, restored.columns["Amount"].TypeCode);
            Assert.AreEqual(1, restored.rows[0].GetField<int>("Id"));
            Assert.AreEqual("alpha", restored.rows[0].GetField<string>("Name"));
            Assert.AreEqual(10.5, restored.rows[0].GetField<double>("Amount"));
            Assert.AreEqual(3, restored.rows[2].GetField<int>("Id"));
            Assert.AreEqual("gamma", restored.rows[2].GetField<string>("Name"));
            Assert.AreEqual(30.75, restored.rows[2].GetField<double>("Amount"));
        }

        [TestMethod]
        public void Header_RoundTrip_PreservesStringValues()
        {
            var original = TestHelpers.CreateSampleTable();

            var restored = TestHelpers.RoundTrip(original, CsvHeaderType.Header);

            Assert.AreEqual(3, restored.columns.Count);
            Assert.AreEqual(3, restored.rows.Count);
            Assert.AreEqual(TypeCode.String, restored.columns["Id"].TypeCode);
            Assert.AreEqual(TypeCode.String, restored.columns["Name"].TypeCode);
            Assert.AreEqual(TypeCode.String, restored.columns["Amount"].TypeCode);
            Assert.AreEqual("1", restored.rows[0].GetFieldAsString("Id"));
            Assert.AreEqual("alpha", restored.rows[0].GetFieldAsString("Name"));
            Assert.AreEqual("10.5", restored.rows[0].GetFieldAsString("Amount"));
        }

        [TestMethod]
        public void None_RoundTrip_PreservesValuesWithGeneratedColumnNames()
        {
            var original = TestHelpers.CreateSampleTable();

            var restored = TestHelpers.RoundTrip(original, CsvHeaderType.None);

            Assert.AreEqual(3, restored.columns.Count);
            Assert.AreEqual(3, restored.rows.Count);
            var columnNames = new System.Collections.Generic.List<string>();
            foreach (IColumn column in restored.columns)
            {
                columnNames.Add(column.Name);
            }
            CollectionAssert.AreEqual(new[] { "Col(0)", "Col(1)", "Col(2)" }, columnNames);
            Assert.AreEqual(TypeCode.String, restored.columns["Col(0)"].TypeCode);
            Assert.AreEqual("1", restored.rows[0].GetFieldAsString("Col(0)"));
            Assert.AreEqual("alpha", restored.rows[0].GetFieldAsString("Col(1)"));
            Assert.AreEqual("10.5", restored.rows[0].GetFieldAsString("Col(2)"));
            Assert.AreEqual("3", restored.rows[2].GetFieldAsString("Col(0)"));
            Assert.AreEqual("gamma", restored.rows[2].GetFieldAsString("Col(1)"));
        }

        [TestMethod]
        public void TypedHeader_EmptyTable_PreservesSchema()
        {
            var original = new DynamicTable();
            original.columns.NewColumn("Id", TypeCode.Int32);
            original.columns.NewColumn("Name", TypeCode.String);

            var restored = TestHelpers.RoundTrip(original, CsvHeaderType.TypedHeader);

            Assert.AreEqual(2, restored.columns.Count);
            Assert.AreEqual(0, restored.rows.Count);
            Assert.AreEqual(TypeCode.Int32, restored.columns["Id"].TypeCode);
            Assert.AreEqual(TypeCode.String, restored.columns["Name"].TypeCode);
        }

        [TestMethod]
        public void TypedHeader_DoubleRoundTrip_RemainsStable()
        {
            var original = TestHelpers.CreateSampleTable();

            var firstPass = TestHelpers.RoundTrip(original, CsvHeaderType.TypedHeader);
            var csv = TestHelpers.WriteToString(firstPass, CsvHeaderType.TypedHeader);
            var secondPass = TestHelpers.ReadFromString(csv, CsvHeaderType.TypedHeader);

            TestHelpers.AssertTablesEqual(firstPass, secondPass);
        }
    }
}