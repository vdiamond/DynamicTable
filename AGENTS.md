# AGENTS.md

Development happens only through Victor and Cursor agents.

## What this repo is

DynamicTable is a GPL-3.0 .NET library suite for in-memory, column-oriented tables: a lightweight alternative to `System.Data.DataTable`, with typed columns, CSV I/O, and extension methods for filter, sort, join, and interop.

The libraries target `net10.0` and are consumed by project reference. There is no root solution, no package metadata, and no CI workflow. The three libraries build, the three MSTest projects pass, and the three console demos run on the .NET 10 SDK.

## Build, run, and test

Run these from the repository root. `dotnet build` and `dotnet test` with no project path fail (`MSB1003`: the root contains no project or solution file).

Build:

```bash
dotnet build Table/Table/Table.csproj
dotnet build TableCsv/TableCsv/TableCsv.csproj
dotnet build TableExtension/TableExtension/TableExtension.csproj
```

`Table` builds with no warnings. `TableCsv` and `TableExtension` succeed and currently emit nullable warnings.

Run the demos:

```bash
dotnet run --project Table/Table_Console/Table_Console.csproj
dotnet run --project TableCsv/TableCsv_Console/TableCsv_Console.csproj
dotnet run --project TableExtension/TableExtension_Console/TableExtension_Console.csproj
```

`TableCsv_Console` writes `..\test.csv` and `..\testr.csv`. On Linux the backslash is part of the file name in the working directory. Delete those files. `TableExtension_Console` prints a placeholder line.

Test (MSTest):

```bash
dotnet test Table/Table_Test/Table_Test.csproj
dotnet test TableCsv/TableCsv_Test/TableCsv_Test.csproj
dotnet test TableExtension/TableExtension_Test/TableExtension_Test.csproj
```

Verified on the .NET 10 SDK: `Table_Test` 68 passed, `TableCsv_Test` 10 passed, `TableExtension_Test` 13 passed.

## Layout

```
DynamicTable/
├── Table/
│   ├── Table/                 core library (namespace Table)
│   ├── Table_Console/         demo
│   └── Table_Test/            MSTest
├── TableCsv/
│   ├── TableCsv/              CSV library (namespace TableCsv)
│   ├── TableCsv_Console/
│   └── TableCsv_Test/
├── TableExtension/
│   ├── TableExtension/        extension methods (namespace TableExtension, Util.cs)
│   ├── TableExtension_Console/
│   └── TableExtension_Test/
├── .cursor/rules/             coding standards
├── AGENTS.md
├── LICENSE                    GPL-3.0
└── README.md
```

Each area has its own `.sln` next to the project. `TableCsv` project-references `Table`. `TableExtension` project-references `Table` and `TableCsv`. Consoles and tests reference the libraries they exercise.

## Dependencies on other vdiamond repos

This repository has no dependency on another vdiamond repo: no sibling `ProjectReference` outside this tree, no git submodule, and no HTTP client.

Known dependents expect this repo cloned as a sibling folder named `DynamicTable`:

```
<parent>/
  DynamicTable/                  https://github.com/vdiamond/DynamicTable
  OptionTools/                   https://github.com/vdiamond/OptionTools
  GeneExpressionProgramming/     https://github.com/vdiamond/GeneExpressionProgramming
```

| Consumer | Project that references DynamicTable | How |
|----------|--------------------------------------|-----|
| [OptionTools](https://github.com/vdiamond/OptionTools) | `option_console/option_console.csproj` | Sibling-folder `ProjectReference` to `..\..\DynamicTable\Table\Table\Table.csproj`, `..\..\DynamicTable\TableCsv\TableCsv\TableCsv.csproj`, and `..\..\DynamicTable\TableExtension\TableExtension\TableExtension.csproj` |
| [GeneExpressionProgramming](https://github.com/vdiamond/GeneExpressionProgramming) | `gep_console/gep_console.csproj` | The same three `..\..\DynamicTable\...` project references |

`option_console` and `gep_console` sit one directory below their repo roots, so `..\..\DynamicTable\...` resolves to this repo beside the consumer. `OptionLib` and `Pgep` do not reference DynamicTable. Public API changes affect both console projects.

## Conventions

`.cursor/rules/coding-standards.mdc` is the governing standard for new and refactored code. Existing code may not yet comply. Bring it into compliance when you touch it.

Observed in this code:

- Column type identity is `TypeCode`. `DynamicTable` owns `Columns` and a `RowCount`. Each read of `rows` builds a new `Rows` view.
- Public interfaces keep lowercase properties `columns` and `rows` (`IDynamicTable`, `IRow`, `IRows`).
- `IColumn` and `IColumn<T>` are separate interfaces. `Column<T>` implements both. `Sort<T>` takes an `IColumn` and casts it to `IColumn<T>`.
- CSV entry points are `CsvReader`, `CsvWriter`, `CsvTable.Read`, `CsvTable.Write`, and `CsvHeaderType` (`None`, `Header`, `TypedHeader`).
- Extra behavior lives in `TableExtension/Util.cs` (`UtilExtensions`, `ColumnTransform`). Change that file in small steps.
- `Table` and `Table_Console` do not set `<Nullable>enable</Nullable>`. The other projects do. There is no root `.editorconfig`, and warnings are not treated as errors.
- Tests use MSTest (`[TestClass]`, `[TestMethod]`). Newer tests are named `MethodName_StateUnderTest_ExpectedBehavior`.
- The three libraries have no NuGet packages. Test projects reference MSTest and `coverlet.collector`.

## What not to touch

- Ask Victor before changing public APIs: namespaces `Table`, `TableCsv`, and `TableExtension`; `columns` and `rows`; `TypeCode` columns; CSV types; extension-method signatures. `option_console` and `gep_console` compile against these projects.
- Keep project names and `.csproj` paths. In-repo references and the dependents above use those relative paths.
- Keep `TypeCode` as the column type identity. Leave `Table` free of framework NuGet packages.
- Leave `Util.cs` as one cohesive file. Edit it in focused steps.
- Keep `LICENSE` (GPL-3.0).
- Do not commit secrets, `bin/`, `obj/`, `.vs/`, or the backslash-named CSV files from `TableCsv_Console`.
