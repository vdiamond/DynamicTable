# DynamicTable

A lightweight .NET library for working with tabular data in memory — like a spreadsheet you can build, filter, join, and save to CSV.

Think of it as a simpler, faster alternative to `System.Data.DataTable`, with strongly typed columns and a small set of focused libraries you can use together or on their own.

---

## What's in this repository?

This project is split into three libraries. Each one builds on the previous layer:

```
Table          →  Core in-memory table (columns + rows)
TableCsv       →  Read and write CSV files into a table
TableExtension →  Extra helpers (sort, join, filter, DataTable interop, and more)
```

| Library | Folder | What it does |
|---------|--------|--------------|
| **Table** | `Table/Table/` | Create and manipulate in-memory tables with typed columns |
| **TableCsv** | `TableCsv/TableCsv/` | Load CSV files into a table and write tables back to CSV |
| **TableExtension** | `TableExtension/` | Convenient extension methods for common data tasks |

All libraries target **.NET 10**.

---

## Core idea: columns and rows

A `DynamicTable` works like a spreadsheet:

- **Columns** have a name and a type (`String`, `Int32`, `Double`, `DateTime`, etc.)
- **Rows** hold one value per column
- All rows share the same column structure

You define the columns first, then add rows:

```csharp
using Table;

var table = new DynamicTable();

// Define columns
table.columns.NewColumn("Name", TypeCode.String);
table.columns.NewColumn("Age", TypeCode.Int32);
table.columns.NewColumn("Score", TypeCode.Double);

// Add rows
var row1 = table.rows.NewRow();
row1.SetField<string>("Name", "Alice");
row1.SetField<int>("Age", 30);
row1.SetField<double>("Score", 95.5);

var row2 = table.rows.NewRow();
row2.UpdateFields(
    new KeyValuePair<string, object>("Name", "Bob"),
    new KeyValuePair<string, object>("Age", 25),
    new KeyValuePair<string, object>("Score", 88.0)
);
```

### Reading values back

You can read data by row or by column:

```csharp
// By row
foreach (IRow row in table.rows)
{
    Console.WriteLine($"{row.GetField<string>("Name")} scored {row.GetField<double>("Score")}");
}

// By column (typed)
var names = (IColumn<string>)table.columns["Name"];
foreach (string name in names)
{
    Console.WriteLine(name);
}
```

### Supported column types

Table supports standard .NET `TypeCode` values:

`Boolean`, `Byte`, `Char`, `DateTime`, `Decimal`, `Double`, `Int16`, `Int32`, `Int64`, `SByte`, `Single`, `String`, `UInt16`, `UInt32`, `UInt64`

---

## TableCsv — working with CSV files

TableCsv connects your in-memory table to CSV files on disk.

### CSV header modes

When reading or writing CSV, you choose how the first row is handled:

| Mode | Description | Example first row |
|------|-------------|---------------------|
| `CsvHeaderType.None` | No header — columns are auto-named `Col(0)`, `Col(1)`, … | *(data starts immediately)* |
| `CsvHeaderType.Header` | First row is column names | `Name,Age,Score` |
| `CsvHeaderType.TypedHeader` | First row is name + type (separated by `@`) | `Name@String,Age@Int32,Score@Double` |

### Writing a table to CSV

```csharp
using Table;
using TableCsv;

var table = new DynamicTable();
table.columns.NewColumn("Product", TypeCode.String);
table.columns.NewColumn("Price", TypeCode.Double);

var row = table.rows.NewRow();
row.SetField<string>("Product", "Widget");
row.SetField<double>("Price", 19.99);

using var writer = new StreamWriter("products.csv");
var csvWriter = new CsvWriter(writer);
CsvTable.Write(csvWriter, table, CsvHeaderType.TypedHeader);
```

Output file:

```
Product@String,Price@Double
Widget,19.99
```

### Reading a CSV file into a table

```csharp
using var reader = new StreamReader("products.csv");
var csvReader = new CsvReader(reader);

var table = new DynamicTable();
CsvTable.Read(csvReader, table, CsvHeaderType.TypedHeader, prefix: null);

Console.WriteLine($"Loaded {table.rows.Count} rows and {table.columns.Count} columns");
```

### Controlling column types when reading

If your CSV has plain headers (no types in the file), you can tell TableCsv what type each column should be:

```csharp
var attrs = new DefaultFieldAttributes();
attrs.Add(new Field("Age", TypeCode.Int32));
attrs.Add(new Field("Name", TypeCode.String));
attrs.SetDefault(TypeCode.String);  // any unlisted column defaults to String

using var reader = new StreamReader("people.csv");
var csvReader = new CsvReader(reader);
var table = new DynamicTable();

CsvTable.Read(csvReader, table, CsvHeaderType.Header, prefix: null, attr: attrs);
```

### Low-level CSV parsing

TableCsv also includes standalone `CsvReader` and `CsvWriter` classes if you just need raw row/column string data without building a full table:

```csharp
using var reader = new StreamReader("data.csv");
var csvReader = new CsvReader(reader);

var row = new List<string>();
while (csvReader.ReadRow(row))
{
    Console.WriteLine(string.Join(" | ", row));
}
```

---

## TableExtension — power tools for your tables

TableExtension adds extension methods to `IDynamicTable`, `IColumn`, `IRow`, and related types. Reference this library when you need more than basic create/read/write.

Add the namespace:

```csharp
using TableExtension;
```

### CSV shortcuts

Shorter versions of the TableCsv read/write calls:

```csharp
// Load from file
var table = new DynamicTable();
table.LoadCsv("input.csv", CsvHeaderType.TypedHeader);

// Write to file
table.WriteCsv("output.csv", CsvHeaderType.TypedHeader);
```

### Filter, sort, and slice

```csharp
// Keep only rows where Age > 25
var adults = table.SelectFrom(r => table.columns["Age"].GetField<int>(r) > 25);

// Sort by Score (ascending) — returns a new table
var sorted = table.Sort<double>((IColumn<double>)table.columns["Score"]);

// Take rows 10–19
var slice = table.GetSubTable(start: 10, length: 10);
```

### Combine tables

```csharp
// Append all rows from another table
table.Append(otherTable);

// Append only rows with new keys (no duplicates)
table.AppendUnique<int>(otherTable, key: "Id");

// Stack tables with matching columns
table.Append(sourceTable, identicalStructure: true);
```

### Joins and lookups

```csharp
// Build a lookup: key value → row index
var keyIndex = mainTable.BuildKey<int>((IColumn<int>)mainTable.columns["Id"]);

// Left-join columns from a lookup table into the main table
mainTable.LeftJoin(
    destKey: keyIndex,
    sourceKey: (IColumn<int>)lookupTable.columns["Id"],
    prefix: "Lookup",
    sourceCols: lookupTable.columns["City"] as IColumn<string>
);
```

### Column operations

```csharp
// Copy only selected columns into a new table (with optional rename)
var subset = table.ColumnSubset(
    prefix: null,
    new ColumnTransform("FirstName", "Name"),
    new ColumnTransform("Age")
);

// Add typed column shorthand
table.columns.NewColumn<int>("Count");

// Get a column as a typed list
var ages = table.AsList<int>("Age");
```

### Work with C# classes and DataTable

```csharp
// Define a simple class
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

// List of objects → table
var people = new List<Person>
{
    new() { Name = "Alice", Age = 30 },
    new() { Name = "Bob", Age = 25 }
};
var table = people.ToTable();

// Table → list of objects
var roundTrip = table.ToClass<Person>();

// Convert to/from System.Data.DataTable
var dataTable = table.ToDataTable();
var backToTable = dataTable.ToTable();
```

### Inspect structure

```csharp
// Create a small metadata table describing column names and types
var schema = table.MetaTable();
// Result: two columns — FieldName, FieldType
```

---

## Project layout

```
DynamicTable/
├── Table/
│   ├── Table/              ← core library
│   ├── Table_Console/      ← simple demo
│   └── Table_Test/         ← unit tests
├── TableCsv/
│   ├── TableCsv/           ← CSV library
│   ├── TableCsv_Console/   ← CSV read/write demo
│   └── TableCsv_Test/      ← unit tests
├── TableExtension/         ← extension methods library
│   ├── TableExtension.csproj
│   ├── TableExtension.sln
│   └── Util.cs
├── .grok/rules/            ← AI assistant project rules
├── LICENSE
└── README.md
```

---

## Building and running

### Build a library

```bash
dotnet build Table/Table/Table.csproj
dotnet build TableCsv/TableCsv/TableCsv.csproj
dotnet build TableExtension/TableExtension.csproj
```

### Run the demo apps

```bash
dotnet run --project Table/Table_Console/Table_Console.csproj
dotnet run --project TableCsv/TableCsv_Console/TableCsv_Console.csproj
```

### Run tests

```bash
dotnet test Table/Table_Test/Table_Test.csproj
dotnet test TableCsv/TableCsv_Test/TableCsv_Test.csproj
```

### Reference in your own project

Add a project reference in your `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="path\to\Table\Table\Table.csproj" />
  <ProjectReference Include="path\to\TableCsv\TableCsv\TableCsv.csproj" />
  <ProjectReference Include="path\to\TableExtension\TableExtension.csproj" />
</ItemGroup>
```

You only need **Table** for basic in-memory work. Add **TableCsv** for file I/O. Add **TableExtension** when you want the helper methods.

---

## When to use DynamicTable

**Good fit:**

- Loading, transforming, and saving CSV data in .NET
- Building tabular datasets in memory without the weight of a full database
- Pipelines that filter, sort, join, or merge tables
- Interop with `DataTable` or plain C# objects

**Not the goal of this library:**

- SQL queries or persistent database storage
- Excel file formats (`.xlsx`)
- Distributed or streaming data at very large scale

---

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE).