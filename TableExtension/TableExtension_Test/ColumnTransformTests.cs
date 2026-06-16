using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableExtension;

namespace TableExtension_Test
{
    [TestClass]
    public class ColumnTransformTests
    {
        [TestMethod]
        public void ColumnSubset_RenamesColumnsAndCopiesValues()
        {
            var source = new DynamicTable();
            source.columns.NewColumn("OldId", TypeCode.Int32);
            source.columns.NewColumn("OldName", TypeCode.String);
            source.rows.NewRow().SetField("OldId", 1);
            source.rows.NewRow().SetField("OldId", 2);
            source.rows[0].SetField("OldName", "alpha");
            source.rows[1].SetField("OldName", "beta");

            var subset = source.ColumnSubset(
                null,
                new ColumnTransform("OldId", "NewId"),
                new ColumnTransform("OldName", "NewName"));

            Assert.AreEqual(2, subset.columns.Count);
            Assert.AreEqual(2, subset.rows.Count);
            Assert.IsTrue(subset.columns.ColumnExists("NewId"));
            Assert.IsTrue(subset.columns.ColumnExists("NewName"));
            Assert.IsFalse(subset.columns.ColumnExists("OldId"));
            Assert.AreEqual(1, subset.rows[0].GetField<int>("NewId"));
            Assert.AreEqual("alpha", subset.rows[0].GetField<string>("NewName"));
            Assert.AreEqual(2, subset.rows[1].GetField<int>("NewId"));
            Assert.AreEqual("beta", subset.rows[1].GetField<string>("NewName"));
        }
    }
}