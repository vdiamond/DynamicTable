using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableExtension;

namespace TableExtension_Test
{
    [TestClass]
    public class JoinTests
    {
        [TestMethod]
        public void BuildKeyAndLeftJoin_AddsMatchedLookupValues()
        {
            var main = TestHelpers.CreateMainTable();
            var lookup = TestHelpers.CreateLookupTable();
            var mainKey = (IColumn<int>)main.columns["Id"];
            var lookupKey = (IColumn<int>)lookup.columns["Id"];
            var lookupScore = (IColumn<int>)lookup.columns["Score"];
            var destKey = main.BuildKey(mainKey);

            main.LeftJoin(destKey, lookupKey, "lkp", lookupScore);

            Assert.IsTrue(main.columns.ColumnExists("lkp.Score"));
            Assert.AreEqual(100, main.rows[0].GetField<int>("lkp.Score"));
            Assert.AreEqual(200, main.rows[1].GetField<int>("lkp.Score"));
            Assert.AreEqual(0, main.rows[2].GetField<int>("lkp.Score"));
        }

        [TestMethod]
        public void LeftJoin_ZeroSourceColumns_Throws()
        {
            var main = TestHelpers.CreateMainTable();
            var lookup = TestHelpers.CreateLookupTable();
            var destKey = main.BuildKey((IColumn<int>)main.columns["Id"]);

            var ex = Assert.ThrowsException<Exception>(() =>
                main.LeftJoin<int, int>(destKey, (IColumn<int>)lookup.columns["Id"], "lkp"));

            Assert.AreEqual("Need to have at least one column to add", ex.Message);
        }

        [TestMethod]
        public void LeftJoin_UnmatchedDouble_IsNaNNotZero()
        {
            var main = TestHelpers.CreateMainTable();
            var lookup = new DynamicTable();
            lookup.columns.NewColumn("Id", TypeCode.Int32);
            lookup.columns.NewColumn("Px", TypeCode.Double);
            var matchedZero = lookup.rows.NewRow();
            matchedZero.SetField("Id", 1);
            matchedZero.SetField("Px", 0.0);
            var matchedValue = lookup.rows.NewRow();
            matchedValue.SetField("Id", 2);
            matchedValue.SetField("Px", 10.5);

            var destKey = main.BuildKey((IColumn<int>)main.columns["Id"]);
            main.LeftJoin(destKey, (IColumn<int>)lookup.columns["Id"], "lkp", (IColumn<double>)lookup.columns["Px"]);

            Assert.AreEqual(0.0, main.rows[0].GetField<double>("lkp.Px"));
            Assert.AreEqual(10.5, main.rows[1].GetField<double>("lkp.Px"));
            Assert.IsTrue(double.IsNaN(main.rows[2].GetField<double>("lkp.Px")));
        }
    }
}