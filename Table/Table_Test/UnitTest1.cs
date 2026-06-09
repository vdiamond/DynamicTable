using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
namespace Table_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod("Base")]
        public void TestMethod1()
        {
            var t = new DynamicTable();
            t.columns.NewColumn("Col1", TypeCode.String);
            t.columns.NewColumn("Col2", TypeCode.Int32);
            t.rows.NewRow().UpdateFields(new KeyValuePair<string, object>("Col1", "111"), new KeyValuePair<string, object>("Col2", 222));
            t.rows.NewRow().UpdateFields(new KeyValuePair<string, object>("Col1", "333"), new KeyValuePair<string, object>("Col2", 444));
            t.rows.NewRow().UpdateFields(new KeyValuePair<string, object>("Col1", "555"), new KeyValuePair<string, object>("Col2", 666));
            t.rows.NewRow().UpdateFields(new KeyValuePair<string, object>("Col1", "777"), new KeyValuePair<string, object>("Col2", 888));

            Assert.AreEqual<int>(2, t.columns.Count);
            Assert.AreEqual<int>(4, t.rows.Count);
        
        }
        [TestMethod("Base2")]
        public void TestMethod2()
        {

        }
    }
}