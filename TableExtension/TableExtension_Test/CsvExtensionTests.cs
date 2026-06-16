using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableCsv;
using TableExtension;

namespace TableExtension_Test
{
    [TestClass]
    public class CsvExtensionTests
    {
        [TestMethod]
        public void LoadCsv_WriteCsv_RoundTripViaExtensions()
        {
            var original = TestHelpers.CreateMainTable();
            original.columns.NewColumn("Amount", TypeCode.Double);
            original.rows[0].SetField("Amount", 10.5);
            original.rows[1].SetField("Amount", 20.25);
            original.rows[2].SetField("Amount", 30.75);

            string csv;
            using (var writer = new StringWriter())
            {
                original.WriteCsv(writer, CsvHeaderType.TypedHeader);
                csv = writer.ToString();
            }

            var restored = new DynamicTable();
            using (var reader = new StringReader(csv))
            {
                restored.LoadCsv(reader, CsvHeaderType.TypedHeader);
            }

            Assert.AreEqual(3, restored.columns.Count);
            Assert.AreEqual(3, restored.rows.Count);
            Assert.AreEqual(1, restored.rows[0].GetField<int>("Id"));
            Assert.AreEqual("alpha", restored.rows[0].GetField<string>("Name"));
            Assert.AreEqual(10.5, restored.rows[0].GetField<double>("Amount"));
            Assert.AreEqual(3, restored.rows[2].GetField<int>("Id"));
            Assert.AreEqual("gamma", restored.rows[2].GetField<string>("Name"));
        }
    }
}