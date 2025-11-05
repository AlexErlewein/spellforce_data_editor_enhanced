# PLAN.md - Spellforce Data Editor Development Plan

## Project Overview
Cross-platform port of the Spellforce Data Editor from Windows Forms to Avalonia UI framework, targeting .NET 8.0.

## Current State (~5-10% Complete)

### ✅ Completed
- Basic Avalonia project setup with SFEngine integration
- MainWindow with Load CFF File button
- CffEditorWindow with three-panel layout (categories, elements, data grid)
- File loading and basic data display using reflection
- Navigation between windows

### ⚠️ Partially Implemented
- Save button exists but only writes debug message
- Basic data binding for display (read-only)
- No editing capabilities yet

### ❌ Major Missing Features (90%+ of work)

#### CFF Editor Core (80% missing)
- **49 Category Forms** (Control1-49): Specialized editors for each data category
- **Undo/Redo System**: Complete URQ implementation with history tracking
- **Search Functionality**: CategorySearchForm with advanced filtering
- **Operator History**: CFFOperatorHistory window for tracking changes
- **References System**: Cross-referencing between data elements
- **Copy/Paste Operations**: Element duplication and transfer
- **Property Validation**: Data validation and constraint enforcement
- **Performance Optimization**: Virtualization for large datasets

#### Map Editor (0% complete)
- MapEditorForm with 3D OpenTK integration
- All MapEdit editors (heightmap, terrain, buildings, units, decorations, etc.)
- Map inspectors and control panels
- Map dialogs and operators
- Terrain texture editing
- Entity placement and editing

#### Asset Management (0% complete)
- SFAssetManagerForm for asset viewing/management
- Asset preview and organization
- Resource handling and optimization

#### Lua/SQL Systems (0% complete)
- Lua dialog builder and editor forms
- SQL editing interface
- Lua decompiler integration
- Script management tools

#### Advanced Features (95% missing)
- Game directory specification and management
- Settings persistence and configuration
- Update checking system
- About dialog and version information
- Debug mode tools (SaveDataEditor)
- Performance monitoring and optimization
- Multi-language support

## Development Roadmap

### Phase 1: Core CFF Editor (2-3 months)
1. **Implement Save Functionality**
   - Add proper file writing logic
   - Data validation before save
   - Error handling and user feedback

2. **Port Critical Category Forms**
   - Start with most commonly used categories
   - Create reusable base form patterns
   - Implement proper data binding

3. **Undo/Redo System**
   - Port URQ (UndoRedoQueue) implementation
   - Add command history tracking
   - Implement memory-efficient command storage

### Phase 2: Enhanced CFF Features (2-3 months)
4. **Search and Navigation**
   - CategorySearchForm implementation
   - Advanced filtering and sorting
   - Quick navigation shortcuts

5. **Data Editing Tools**
   - Copy/paste operations
   - Bulk editing capabilities
   - Data validation and constraints

6. **Performance Optimization**
   - Virtualization for large datasets
   - Lazy loading of categories
   - Memory usage optimization

### Phase 3: Map Editor (4-6 months)
7. **Map Editor Foundation**
   - Basic map loading and display
   - OpenTK integration for 3D rendering
   - Camera controls and navigation

8. **Map Editing Tools**
   - Heightmap editor
   - Terrain texture editor
   - Entity placement (buildings, units, decorations)

9. **Map Inspectors**
   - Property panels for different entity types
   - Real-time editing capabilities

### Phase 4: Asset Management (1-2 months)
10. **Asset Manager**
    - Asset browsing and organization
    - Preview capabilities
    - Import/export functionality

### Phase 5: Lua/SQL Integration (1-2 months)
11. **Script Editing Tools**
    - Lua dialog builder
    - SQL interface
    - Decompiler integration

### Phase 6: Polish and Production (1 month)
12. **Final Features**
    - Settings management
    - Update checking
    - Documentation and testing

## Technical Challenges
- **49 Category Forms**: Each requires individual porting from WinForms to Avalonia
- **Complex Controls**: Many specialized WinForms controls need Avalonia equivalents
- **3D Integration**: Map editor requires careful OpenTK integration with Avalonia
- **Performance**: Handling large datasets efficiently with Avalonia patterns
- **Data Binding**: Converting WinForms patterns to Avalonia MVVM-style binding

## Success Metrics
- Feature parity with Windows Forms version
- Cross-platform compatibility (Windows, macOS, Linux)
- Performance comparable or better than original
- Maintained data integrity and compatibility

## Estimated Timeline
**Total: 10-16 months** with focused full-time development effort.