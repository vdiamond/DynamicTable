using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Table;
using TableExtension;

namespace TableExtension_Test
{
    [TestClass]
    public class InteropTests
    {
        [TestMethod]
        public void ToDataTable_ToTable_RoundTripPreservesValues()
        {
            var original = new DynamicTable();
            original.columns.NewColumn("Id", TypeCode.Int32);
            original.columns.NewColumn("Name", TypeCode.String);
            original.columns.NewColumn("Amount", TypeCode.Double);
            TestHelpers.AddRow(original, 1, "alpha");
            var row = original.rows.NewRow();
            row.SetField("Id", 2);
            row.SetField("Name", "beta");
            row.SetField("Amount", 42.5);

            DataTable dataTable = original.ToDataTable();
            IDynamicTable restored = dataTable.ToTable();

            Assert.AreEqual(3, restored.columns.Count);
            Assert.AreEqual(2, restored.rows.Count);
            Assert.AreEqual(1, restored.rows[0].GetField<int>("Id"));
            Assert.AreEqual("alpha", restored.rows[0].GetField<string>("Name"));
            Assert.AreEqual(2, restored.rows[1].GetField<int>("Id"));
            Assert.AreEqual("beta", restored.rows[1].GetField<string>("Name"));
            Assert.AreEqual(42.5, restored.rows[1].GetField<double>("Amount"));
        }

        [TestMethod]
        public void GetStructure_ToTable_ToClass_RoundTripsPocoList()
        {
            var people = new List<TestPerson>
            {
                new TestPerson { Id = 1, Name = "alpha", Amount = 10.5 },
                new TestPerson { Id = 2, Name = "beta", Amount = 20.25 }
            };

            IDynamicTable? structure = people[0].GetStructure<TestPerson>();
            Assert.IsNotNull(structure);

            IDynamicTable table = people.ToTable<TestPerson>();
            IList<TestPerson> restored = table.ToClass<TestPerson>();

            Assert.AreEqual(2, restored.Count);
            Assert.AreEqual(1, restored[0].Id);
            Assert.AreEqual("alpha", restored[0].Name);
            Assert.AreEqual(10.5, restored[0].Amount);
            Assert.AreEqual(2, restored[1].Id);
            Assert.AreEqual("beta", restored[1].Name);
            Assert.AreEqual(20.25, restored[1].Amount);
        }
    }
}