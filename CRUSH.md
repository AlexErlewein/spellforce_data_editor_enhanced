# CRUSH.md - Spellforce Data Editor

## Build Commands
```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build SpellforceDataEditor.Avalonia/SpellforceDataEditor.Avalonia.csproj

# Run Avalonia app
dotnet run --project SpellforceDataEditor.Avalonia
```

## Code Style Guidelines

### Naming Conventions
- **Classes**: PascalCase (e.g., `MainForm`, `SFMap`)
- **Methods**: PascalCase (e.g., `Load()`, `Save()`)
- **Variables**: camelCase (local), underscore_prefix (private fields)
- **Interfaces**: Prefix with 'I' (e.g., `ICategory`)
- **Constants**: PascalCase or UPPER_SNAKE_CASE

### Formatting
- 4-space indentation
- K&R brace style (`{` on same line)
- One class per file
- Use partial classes for forms

### Import Organization
- System imports first, then project-specific
- Alphabetical ordering within groups
- Remove unused imports

### Error Handling
- Use try-catch blocks for exception handling
- Log via `LogUtils.Log` with Info/Warning/Error methods
- Some methods return error codes (0 = success, non-zero = error)

### Type Usage
- `var` for local variables when type is obvious
- Explicit types for public APIs and complex cases
- SFEngine uses `unsafe` code for performance-critical operations
- Use `Span<T>` and `MemoryMarshal` for memory-efficient operations

### Performance Considerations
- SFEngine uses unsafe blocks and pointer arithmetic for binary data
- Memory pooling for high-frequency allocations
- Struct layouts with explicit packing for serialization
- Reflection for dynamic field access in UI components

### Project Structure
- **SFEngine**: Core logic and data manipulation
- **SpellforceDataEditor**: Windows Forms UI
- **SpellforceDataEditor.Avalonia**: Cross-platform Avalonia UI
- Follow namespace hierarchy in directory structure