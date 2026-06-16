using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;

namespace Table_Test
{
    [TestClass]
    public class TypeSafetyTests
    {
        [TestMethod]
        public void SetFieldAsObject_GetFieldAsObject_RoundTripsBoxedValue()
        {
            var table = TestHelpers.CreateTable(("Amount", TypeCode.Int32), ("Label", TypeCode.String));
            var row = table.rows.NewRow();

            row.SetFieldAsObject("Amount", 42);
            row.SetFieldAsObject("Label", "value");

            Assert.AreEqual(42, row.GetFieldAsObject("Amount"));
            Assert.AreEqual("value", row.GetFieldAsObject("Label"));
        }

        [TestMethod]
        public void GetField_WrongGenericType_ThrowsInvalidCast()
        {
            var table = TestHelpers.CreateTable(("Amount", TypeCode.Int32));
            TestHelpers.AddRow(table, ("Amount", 42));

            Assert.ThrowsException<InvalidCastException>(() => table.rows[0].GetField<string>("Amount"));
        }

        [TestMethod]
        public void SetFieldAsString_InvalidParse_ThrowsFormatException()
        {
            var table = TestHelpers.CreateTable(("Amount", TypeCode.Int32));
            var row = table.rows.NewRow();

            Assert.ThrowsException<FormatException>(() => row.SetFieldAsString("Amount", "not-a-number"));
        }

        [TestMethod]
        public void SetFieldAsObject_WrongBoxedType_ThrowsInvalidCast()
        {
            var table = TestHelpers.CreateTable(("Amount", TypeCode.Int32));
            var row = table.rows.NewRow();

            Assert.ThrowsException<InvalidCastException>(() => row.SetFieldAsObject("Amount", "not-an-int"));
        }
    }
}