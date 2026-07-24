# Architecture for DynamicTable

This repository is a C# library suite for in-memory, column-oriented tabular data. It provides a lightweight alternative to `DataTable` with typed columns, row operations, CSV I/O, and extension methods for joins, sorting, and interop.

## Repository Layout

| Folder | Purpose |
|--------|---------|
| `Table/Table/` | Core library — `DynamicTable`, `IColumn`, `IRow`, `IDynamicTable`, etc. |
| `TableCsv/TableCsv/` | CSV read/write built on the core `Table` library |
| `TableExtension/TableExtension/` | Extension methods (`UtilExtensions`) for CSV loading, joins, sorting, `DataTable` interop, and POCO mapping |

Each component has its own solution (`.sln`), plus optional `_Test` (xUnit) and `_Console` projects. There is no root-level solution — build the relevant project or solution for the area you are changing.

## Design Principles

- Prefer extending the existing interface-driven design (`IDynamicTable`, `IColumn<T>`, `IRow`, `IColumns`, `IRows`) rather than introducing parallel abstractions.
- Keep libraries free of console/test code; sample usage belongs in `*_Console` projects, validation in `*_Test` projects.
- Match the existing code style: explicit `TypeCode` handling, lowercase property names on interfaces (`columns`, `rows`), and `#region private` blocks where used.
- Preserve backward compatibility when changing public APIs — this library is referenced by other projects outside this repo.
- Use `TypeCode` (not `System.Type`) for column type identity — this is central to the design.
- Generic column access goes through `IColumn<T>` and `GetField<T>` / `SetField<T>` patterns.
- Avoid adding heavy external dependencies; the core `Table` project has zero NuGet dependencies.

## Component Architecture

- **`DynamicTable`** owns a `Columns` collection and `RowCount`; `rows` is a view constructed on access.
- **Column typing** is enforced via `TypeCode` at creation time (`NewColumn(name, typeCode)`).
- **`TableCsv`** provides `CsvReader`/`CsvWriter`, `CsvTable.Read`/`Write`, and header-type handling via `CsvHeaderType`.
- **`TableExtension`** adds extension methods on `IDynamicTable`, `IColumn`, `IColumns`, and `IRow`/`IRows` — keep new helpers here rather than bloating the core library unless they are fundamental primitives.
- Interop helpers (`ToDataTable`, `ToTable`, `ToClass<T>`, `ToTable<T>`) use reflection over POCO properties matched by name and `TypeCode`.

## Architectural Constraints

- Do not replace `TypeCode`-based columns with `object` or `dynamic` columns.
- Do not add framework-specific dependencies to the core `Table` library.
- Do not rewrite `Util.cs` wholesale — it is large but cohesive; make focused, incremental edits.
- Do not rename `TableExtension` or move projects without updating all `ProjectReference` paths.
