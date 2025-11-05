# Project Overview

This project is a data editor for Spellforce games, primarily focused on editing `Gamedata.cff` files. It consists of several C# projects:

*   **`SpellforceDataEditor`**: The original Windows Forms application for editing game data.
*   **`SpellforceDataEditor.Avalonia`**: An ongoing effort to create a cross-platform version of the data editor using the Avalonia UI framework. This project aims to replicate the functionality of the Windows Forms application on multiple operating systems.
*   **`SFEngine`**: A core library containing the logic for reading, writing, and manipulating Spellforce game data (e.g., CFF files, map data, 3D models). Both `SpellforceDataEditor` and `SpellforceDataEditor.Avalonia` depend on this library.
*   **`MapViewerNetNative`**: (Purpose not fully determined from initial scan, but appears to be related to map viewing with native interop).

The primary goal of the recent work has been to advance the `SpellforceDataEditor.Avalonia` project, specifically by implementing the CFF editor window to load and display data from `Gamedata.cff` files.

# Building and Running

The project uses .NET 8.0.

## Building

To build the entire solution, navigate to the root directory and run:

```bash
dotnet build
```

To build a specific project (e.g., the Avalonia project), navigate to its directory and run:

```bash
dotnet build
```

## Running `SpellforceDataEditor.Avalonia`

1.  Build the `SpellforceDataEditor.Avalonia` project.
2.  Navigate to the output directory (e.g., `SpellforceDataEditor.Avalonia/bin/Debug/net8.0/`).
3.  Run the executable:

    ```bash
    dotnet run
    ```

    (Note: You might need to specify the full path to the executable depending on your shell and environment setup.)

4.  From the main window, click "Load CFF File" and select a `Gamedata.cff` file. The editor will then display categories and elements from the file.

# Development Conventions

*   **Language**: C#
*   **Framework**: .NET 8.0
*   **UI Frameworks**:
    *   `SpellforceDataEditor`: Windows Forms
    *   `SpellforceDataEditor.Avalonia`: Avalonia UI
*   **Core Logic**: Resides in the `SFEngine` project, promoting separation of concerns.
*   **Dependencies**: NuGet packages are managed via `.csproj` files. Notable UI-related dependencies in `SpellforceDataEditor.Avalonia` include `Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`, `MessageBox.Avalonia`, and `Avalonia.Controls.DataGrid`.
*   **Unsafe Code**: The `SFEngine` project utilizes `AllowUnsafeBlocks` for performance-critical operations, particularly in data manipulation.
*   **File Handling**: The `SFEngine` library handles the low-level reading and writing of `.cff` files.
*   **Event Handling**: UI events are subscribed to in the code-behind files (e.g., `MainWindow.axaml.cs`, `CffEditorWindow.axaml.cs`).
*   **Data Display**: `DataGrid` is used in the Avalonia project to display item properties dynamically using reflection.
