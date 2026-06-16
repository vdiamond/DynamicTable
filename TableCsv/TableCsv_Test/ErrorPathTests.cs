using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableCsv;

namespace TableCsv_Test
{
    [TestClass]
    public class ErrorPathTests
    {
        [TestMethod]
        public void Read_EmptyStream_Throws()
        {
            var table = new DynamicTable();
            using var reader = new StringReader(string.Empty);

            var ex = Assert.ThrowsException<System.Exception>(() =>
                CsvTable.Read(new CsvReader(reader), table, CsvHeaderType.TypedHeader, ""));

            Assert.AreEqual("Error reading table - stream empty", ex.Message);
        }
    }
}