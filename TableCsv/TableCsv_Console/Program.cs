// See https://aka.ms/new-console-template for more information
using Table;
using TableCsv;
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

var tab = new DynamicTable();
tab.columns.NewColumn("id1", TypeCode.Int32);
tab.columns.NewColumn("id2", TypeCode.String);
tab.columns.NewColumn("id3", TypeCode.String);
tab.columns.NewColumn("id4", TypeCode.String);
tab.columns.NewColumn("id5", TypeCode.Double);
tab.columns.NewColumn("id6", TypeCode.Double);
tab.columns.NewColumn("id7", TypeCode.Double);
tab.columns.NewColumn("id8", TypeCode.Double);
for (int i = 0; i < 10; i++) {  TestCreateRow(tab, i); }

using (var sw = new StreamWriter(@"..\test.csv"))
{
    CsvTable.Write(new CsvWriter(sw), tab, CsvHeaderType.TypedHeader);
}

var rtab = new DynamicTable();
using (var sr = new StreamReader(@"..\test.csv"))
{
    CsvTable.Read(new CsvReader(sr), rtab, CsvHeaderType.TypedHeader, "");
}

using (var swOut = new StreamWriter(@"..\testr.csv"))
{
    CsvTable.Write(new CsvWriter(swOut), rtab, CsvHeaderType.TypedHeader);
}

