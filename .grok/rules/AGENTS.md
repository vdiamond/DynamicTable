# Project Rules for DynamicTable

This repository is a C# library suite for in-memory, column-oriented tabular data. It provides a lightweight alternative to `DataTable` with typed columns, row operations, CSV I/O, and extension methods for joins, sorting, and interop.

## Repository Layout

| Folder | Purpose |
|--------|---------|
| `Table/Table/` | Core library — `DynamicTable`, `IColumn`, `IRow`, `IDynamicTable`, etc. |
| `TableCsv/TableCsv/` | CSV read/write built on the core `Table` library |
| `TableExtension/` | Extension methods (`UtilExtensions`) for CSV loading, joins, sorting, `DataTable` interop, and POCO mapping |

Each component has its own solution (`.sln`), plus optional `_Test` (xUnit) and `_Console` projects. There is no root-level solution — build the relevant project or solution for the area you are changing.

## General Guidelines

- Prefer extending the existing interface-driven design (`IDynamicTable`, `IColumn<T>`, `IRow`, `IColumns`, `IRows`) rather than introducing parallel abstractions.
- Keep libraries free of console/test code; sample usage belongs in `*_Console` projects, validation in `*_Test` projects.
- Match the existing code style: explicit `TypeCode` handling, lowercase property names on interfaces (`columns`, `rows`), and `#region private` blocks where used.
- Preserve backward compatibility when changing public APIs — this library is referenced by other projects outside this repo.

## Coding Standards (C# / .NET)

- Target **net10.0** (.NET 10) exclusively for all new and updated projects — do not target earlier framework versions.
- Use standard C# naming: PascalCase for types and public members, camelCase for locals and parameters.
- Always use opening '{' and closing '}' braces for 'if' , 'else', 'do', 'for', 'foreach', 'while' and any other language constructs that expect 0 or more lines of code.
- Never use the the 'goto' statement
- For classes Class members must be defined in this exact sequence—no deviations:
    1) Constructors
    2) All public members
    3) All protected members, enclosed in a region named "protected"
    4) All private members, enclosed in a region named "private". Private member names must be prefixed with "_".   
- Prefer `var` for obvious local types; use explicit types in public APIs.
- Use `TypeCode` (not `System.Type`) for column type identity — this is central to the design.
- Generic column access goes through `IColumn<T>` and `GetField<T>` / `SetField<T>` patterns.
- Throw descriptive `Exception` messages for structural mismatches (column counts, type mismatches, mapping lengths) — follow existing messages in `Util.cs` and core types.
- Add XML doc comments when introducing new public types or non-obvious public methods in libraries.
- Avoid adding heavy external dependencies; the core `Table` project has zero NuGet dependencies.

## Build, Test & Run

Build a specific library:

```bash
dotnet build Table/Table/Table.csproj
dotnet build TableCsv/TableCsv/TableCsv.csproj
dotnet build TableExtension/TableExtension.csproj
```

Run tests:

```bash
dotnet test Table/Table_Test/Table_Test.csproj
dotnet test TableCsv/TableCsv_Test/TableCsv_Test.csproj
```

Run console demos:

```bash
dotnet run --project Table/Table_Console/Table_Console.csproj
dotnet run --project TableCsv/TableCsv_Console/TableCsv_Console.csproj
```

Prefer `dotnet build -c Release` before validating performance-sensitive changes.

## Architecture Notes

- **`DynamicTable`** owns a `Columns` collection and `RowCount`; `rows` is a view constructed on access.
- **Column typing** is enforced via `TypeCode` at creation time (`NewColumn(name, typeCode)`).
- **`TableCsv`** provides `CsvReader`/`CsvWriter`, `CsvTable.Read`/`Write`, and header-type handling via `CsvHeaderType`.
- **`TableExtension`** adds extension methods on `IDynamicTable`, `IColumn`, `IColumns`, and `IRow`/`IRows` — keep new helpers here rather than bloating the core library unless they are fundamental primitives.
- Interop helpers (`ToDataTable`, `ToTable`, `ToClass<T>`, `ToTable<T>`) use reflection over POCO properties matched by name and `TypeCode`.

## Version Control & Workflow

- Use conventional commit prefixes when possible (`feat:`, `fix:`, `refactor:`, `test:`, `docs:`).
- Do not commit `bin/`, `obj/`, or `.vs/` directories.
- Do not rename `TableExtension` or move projects without updating all `ProjectReference` paths.

## Working with Grok

- Before implementing features, read the relevant interfaces in `Table/Table/ITable.cs` and the concrete types they bind to.
- When adding table operations, check whether an extension already exists in `TableExtension/Util.cs` before creating duplicates.
- For CSV changes, coordinate updates across `TableCsv` parsers and any `LoadCsv`/`WriteCsv` extensions.
- Run the appropriate `dotnet test` project after changes to core column/row behavior or CSV parsing.
- Use `grok inspect` to verify loaded project rules.

## Things to Avoid

- Do not replace `TypeCode`-based columns with `object` or `dynamic` columns.
- Do not add framework-specific dependencies to the core `Table` library.
- Do not rewrite `Util.cs` wholesale — it is large but cohesive; make focused, incremental edits.
- Do not create markdown documentation files unless requested.