using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;

namespace Table_Test
{
    [TestClass]
    public class ColumnTests
    {
        [DataTestMethod]
        [DataRow(TypeCode.Boolean)]
        [DataRow(TypeCode.Byte)]
        [DataRow(TypeCode.Char)]
        [DataRow(TypeCode.DateTime)]
        [DataRow(TypeCode.Decimal)]
        [DataRow(TypeCode.Double)]
        [DataRow(TypeCode.Int16)]
        [DataRow(TypeCode.Int32)]
        [DataRow(TypeCode.Int64)]
        [DataRow(TypeCode.SByte)]
        [DataRow(TypeCode.Single)]
        [DataRow(TypeCode.String)]
        [DataRow(TypeCode.UInt16)]
        [DataRow(TypeCode.UInt32)]
        [DataRow(TypeCode.UInt64)]
        public void TypedFieldAccess_RoundTripsViaRowAndColumn(TypeCode typeCode)
        {
            var table = TestHelpers.CreateTable(("Value", typeCode));
            var expected = TestHelpers.SampleValue(typeCode);
            var row = table.rows.NewRow();

            row.SetFieldAsObject("Value", expected);

            var column = table.columns["Value"];
            Assert.AreEqual(expected, column.GetField(0));
            Assert.AreEqual(expected, row.GetFieldAsObject("Value"));
            Assert.AreEqual(expected, ((IList)column)[0]);
        }

        [DataTestMethod]
        [DataRow(TypeCode.Boolean)]
        [DataRow(TypeCode.Double)]
        [DataRow(TypeCode.Int32)]
        [DataRow(TypeCode.String)]
        public void StringConversion_RoundTripsForCommonTypes(TypeCode typeCode)
        {
            var table = TestHelpers.CreateTable(("Value", typeCode));
            var row = table.rows.NewRow();
            var stringValue = TestHelpers.SampleString(typeCode);

            row.SetFieldAsString("Value", stringValue);

            if (typeCode == TypeCode.DateTime)
            {
                Assert.AreEqual(new DateTime(2024, 6, 15, 10, 30, 0), row.GetField<DateTime>("Value"));
            }
            else if (typeCode == TypeCode.Boolean)
            {
                Assert.IsTrue(row.GetField<bool>("Value"));
                Assert.AreEqual("True", row.GetFieldAsString("Value"));
            }
            else
            {
                Assert.AreEqual(stringValue, row.GetFieldAsString("Value"));
            }
        }

        [TestMethod]
        public void ColumnExists_ReturnsTrueForExistingAndFalseForMissing()
        {
            var table = TestHelpers.CreateTable(("Present", TypeCode.Int32));

            Assert.IsTrue(table.columns.ColumnExists("Present"));
            Assert.IsFalse(table.columns.ColumnExists("Missing"));
            Assert.AreEqual("Present", table.columns["Present"].Name);
        }

        [TestMethod]
        public void ColumnEnumeration_YieldsAllRowValues()
        {
            var table = TestHelpers.CreateTable(("A", TypeCode.Int32), ("B", TypeCode.String));
            TestHelpers.AddRow(table, ("A", 10), ("B", "ten"));
            TestHelpers.AddRow(table, ("A", 20), ("B", "twenty"));

            var columnA = table.columns["A"];
            var values = new System.Collections.Generic.List<object>();
            foreach (var value in columnA)
            {
                values.Add(value);
            }

            CollectionAssert.AreEqual(new object[] { 10, 20 }, values);
        }

        [TestMethod]
        public void ColumnIListIndexer_GetAndSetValues()
        {
            var table = TestHelpers.CreateTable(("Name", TypeCode.String), ("Count", TypeCode.Int32));
            table.rows.NewRow();
            table.rows.NewRow();

            IList nameColumn = (IList)table.columns["Name"];
            IList countColumn = (IList)table.columns["Count"];

            nameColumn[0] = "alpha";
            nameColumn[1] = "beta";
            countColumn[0] = 7;
            countColumn[1] = 8;

            Assert.AreEqual("alpha", table.rows[0].GetField<string>("Name"));
            Assert.AreEqual("beta", table.rows[1].GetField<string>("Name"));
            Assert.AreEqual(7, table.rows[0].GetField<int>("Count"));
            Assert.AreEqual(8, table.rows[1].GetField<int>("Count"));
        }

        [DataTestMethod]
        [DataRow(TypeCode.Boolean)]
        [DataRow(TypeCode.Byte)]
        [DataRow(TypeCode.Char)]
        [DataRow(TypeCode.DateTime)]
        [DataRow(TypeCode.Decimal)]
        [DataRow(TypeCode.Double)]
        [DataRow(TypeCode.Int16)]
        [DataRow(TypeCode.Int32)]
        [DataRow(TypeCode.Int64)]
        [DataRow(TypeCode.SByte)]
        [DataRow(TypeCode.Single)]
        [DataRow(TypeCode.String)]
        [DataRow(TypeCode.UInt16)]
        [DataRow(TypeCode.UInt32)]
        [DataRow(TypeCode.UInt64)]
        public void NewColumn_SucceedsForAllSupportedTypeCodes(TypeCode typeCode)
        {
            var table = new DynamicTable();
            var column = table.columns.NewColumn(typeCode.ToString(), typeCode);

            Assert.IsNotNull(column);
            Assert.AreEqual(typeCode, column.TypeCode);
            Assert.AreEqual(typeCode.ToString(), column.Name);
        }
    }
}