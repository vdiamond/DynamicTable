using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TableCsv;

namespace TableCsv_Test
{
    [TestClass]
    public class CsvParserTests
    {
        [TestMethod]
        public void ReadRow_ParsesQuotedFieldContainingComma()
        {
            using var reader = new StringReader("\"a,b\",c");
            var csvReader = new CsvReader(reader);
            var columns = new List<string>();

            Assert.IsTrue(csvReader.ReadRow(columns));
            CollectionAssert.AreEqual(new[] { "a,b", "c" }, columns);
            Assert.IsFalse(csvReader.ReadRow(columns));
        }

        [TestMethod]
        public void ReadRow_ParsesEscapedQuotes()
        {
            using var reader = new StringReader("\"say \"\"hi\"\"\",x");
            var csvReader = new CsvReader(reader);
            var columns = new List<string>();

            Assert.IsTrue(csvReader.ReadRow(columns));
            CollectionAssert.AreEqual(new[] { "say \"hi\"", "x" }, columns);
        }

        [TestMethod]
        public void WriteRow_QuotesFieldsWithSpecialCharacters()
        {
            using var writer = new StringWriter();
            var csvWriter = new CsvWriter(writer);

            csvWriter.WriteRow(new List<string> { "plain", "a,b", "say \"hi\"" });

            Assert.AreEqual("plain,\"a,b\",\"say \"\"hi\"\"\"\r\n", writer.ToString());
        }

        [TestMethod]
        public void ReadAll_ReturnsFullGrid()
        {
            const string csv = "h1,h2\r\nv1,v2\r\nv3,v4\r\n";
            using var reader = new StringReader(csv);

            var grid = CsvReader.ReadAll(reader);

            Assert.IsNotNull(grid);
            Assert.AreEqual(3, grid!.Count);
            CollectionAssert.AreEqual(new[] { "h1", "h2" }, grid[0]);
            CollectionAssert.AreEqual(new[] { "v1", "v2" }, grid[1]);
            CollectionAssert.AreEqual(new[] { "v3", "v4" }, grid[2]);
        }
    }
}