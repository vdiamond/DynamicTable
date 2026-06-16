using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;

namespace Table_Test
{
    [TestClass]
    public class ErrorPathTests
    {
        [DataTestMethod]
        [DataRow(TypeCode.Empty, "Empty: TypeCode not supported")]
        [DataRow(TypeCode.DBNull, "DBNull: TypeCode not supported")]
        [DataRow(TypeCode.Object, "Object: TypeCode not supported")]
        public void NewColumn_UnsupportedTypeCode_Throws(TypeCode typeCode, string expectedMessage)
        {
            var table = new DynamicTable();

            var ex = Assert.ThrowsException<Exception>(() => table.columns.NewColumn("Bad", typeCode));
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [TestMethod]
        public void NewColumn_NullName_Throws()
        {
            var table = new DynamicTable();

            var ex = Assert.ThrowsException<Exception>(() => table.columns.NewColumn(null!, TypeCode.String));
            Assert.AreEqual("Column name cannot be NULL", ex.Message);
        }

        [TestMethod]
        public void NewColumn_DuplicateName_Throws()
        {
            var table = TestHelpers.CreateTable(("Duplicate", TypeCode.Int32));

            Assert.ThrowsException<ArgumentException>(() => table.columns.NewColumn("Duplicate", TypeCode.String));
        }

        [TestMethod]
        public void ColumnIndexer_MissingName_ThrowsKeyNotFound()
        {
            var table = TestHelpers.CreateTable(("Exists", TypeCode.Int32));

            Assert.ThrowsException<KeyNotFoundException>(() => { var _ = table.columns["Missing"]; });
        }

        [TestMethod]
        public void RowGetField_MissingColumn_ThrowsKeyNotFound()
        {
            var table = TestHelpers.CreateTable(("Exists", TypeCode.Int32));
            table.rows.NewRow();

            Assert.ThrowsException<KeyNotFoundException>(() => table.rows[0].GetField<int>("Missing"));
        }

        [DataTestMethod]
        [DataRow(-1, 1)]
        [DataRow(0, 0)]
        public void RemoveRows_InvalidIndexOrCount_Throws(int index, int count)
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Id", 1));

            var ex = Assert.ThrowsException<Exception>(() => table.rows.RemoveRows(index, count));
            Assert.AreEqual("For RemoveRows, index >=0 and count >=1", ex.Message);
        }

        [TestMethod]
        public void RemoveRows_OutOfRange_Throws()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Id", 1));
            TestHelpers.AddRow(table, ("Id", 2));

            var ex = Assert.ThrowsException<Exception>(() => table.rows.RemoveRows(1, 2));
            Assert.AreEqual("For RemoveRows, indexand count out of range", ex.Message);
        }

        [TestMethod]
        public void RowsInsert_ThrowsNotSupported()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Id", 1));

            Assert.ThrowsException<NotSupportedException>(() =>
                ((IList<IRow>)table.rows).Insert(0, table.rows[0]));
        }

        [TestMethod]
        public void RowsAdd_ThrowsInvalidOperation()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            var row = table.rows.NewRow();

            var ex = Assert.ThrowsException<Exception>(() => ((ICollection<IRow>)table.rows).Add(row));
            Assert.AreEqual("Invalid operation on DynamicTable column", ex.Message);
        }

        [TestMethod]
        public void ColumnGenericAdd_ThrowsInvalidOperation()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            var column = (IColumn<int>)table.columns["Id"];

            var ex = Assert.ThrowsException<Exception>(() => ((ICollection<int>)column).Add(1));
            Assert.AreEqual("Invalid operation on DynamicTable column", ex.Message);
        }

        [TestMethod]
        public void RowIndexerSet_Throws()
        {
            var table = TestHelpers.CreateTable(("Id", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Id", 1));

            var ex = Assert.ThrowsException<Exception>(() => table.rows[0] = table.rows[0]);
            Assert.AreEqual("Row cannot be set in DynamicTable", ex.Message);
        }
    }
}