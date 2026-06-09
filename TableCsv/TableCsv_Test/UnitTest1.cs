using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Table;
using TableCsv;
namespace TableCsv_Test
{
    [TestClass]
    public class UnitTest1
    {
        void TestCreateRow(IDynamicTable table, int id)
        {
            var row = table.rows.NewRow();
            row.SetField<int>("id1", id);
            row.SetField<string>("id2", "id2" + id.ToString());
            row.SetField<string>("id3", "id3" + id.ToString());
            row.SetField<string>("id4", "id4" + id.ToString());
            row.SetField<double>("id5", id * 5.0);
            row.SetField<double>("id6", id * 6.0);
            row.SetField<double>("id7", id * 7.0);
            row.SetField<double>("id8", id * 8.0);
        }


        [TestMethod("Base")]
        public void TestMethod1()
        {

            var tab = new DynamicTable();
            tab.columns.NewColumn("id1", TypeCode.Int32);
            tab.columns.NewColumn("id2", TypeCode.String);
            tab.columns.NewColumn("id3", TypeCode.String);
            tab.columns.NewColumn("id4", TypeCode.String);
            tab.columns.NewColumn("id5", TypeCode.Double);
            tab.columns.NewColumn("id6", TypeCode.Double);
            tab.columns.NewColumn("id7", TypeCode.Double);
            tab.columns.NewColumn("id8", TypeCode.Double);
            for (int i = 0; i < 10; i++) { TestCreateRow(tab, i); }

            var fw = new CsvFileWriter(@"..\test.csv");
            CsvTable.Write(fw, tab, CsvHeaderType.TypedHeader);
            fw.Dispose();

            var fr = new CsvFileReader(@"..\test.csv");
            var rtab = new DynamicTable();
            CsvTable.Read(fr, rtab, CsvHeaderType.TypedHeader, "");
            fr.Dispose();

            // do the checks
            Assert.AreEqual<int>(8, rtab.columns.Count);
            Assert.AreEqual<int>(10, rtab.rows.Count);

            var fwo = new CsvFileWriter(@"..\testr.csv");
            CsvTable.Write(fwo, rtab, CsvHeaderType.TypedHeader);
            fwo.Dispose();

        }
    }
}