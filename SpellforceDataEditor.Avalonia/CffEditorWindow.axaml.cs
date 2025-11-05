using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SFEngine.SFCFF;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using SFEngine.SFCFF.CTG;

namespace SpellforceDataEditor.Avalonia
{
    public class EditableProperty : INotifyPropertyChanged
    {
        private string _propertyName;
        private object _value;
        private FieldInfo? _fieldInfo;
        private SFEngine.SFCFF.ICategory? _category;
        private int _itemIndex;
        private int _subIndex;

        public string PropertyName
        {
            get => _propertyName;
            set
            {
                _propertyName = value;
                OnPropertyChanged(nameof(PropertyName));
            }
        }

        public object Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                     // Use the category's SetField method instead of direct field access
                     if (_category != null && _fieldInfo != null)
                     {
                         try
                         {
                             // Use reflection to call SetField on the category
                             var setFieldMethod = _category.GetType().GetMethod("SetField");
                             if (setFieldMethod != null)
                             {
                                 // Make the method generic with the field type
                                 var genericMethod = setFieldMethod.MakeGenericMethod(_fieldInfo.FieldType);

                                  // Check if this is CategoryBaseMultiple<T> (different signature)
                                  var categoryType = _category.GetType();
                                  if (categoryType.BaseType != null && categoryType.BaseType.IsGenericType &&
                                      categoryType.BaseType.GetGenericTypeDefinition() == typeof(SFEngine.SFCFF.CategoryBaseMultiple<>))
                                  {
                                      // For CategoryBaseMultiple<T>: SetField<U>(int index, int subindex, string field_name, U value)
                                      int subIndexToUse = _subIndex >= 0 ? _subIndex : 0;
                                      genericMethod.Invoke(_category, new object[] { _itemIndex, subIndexToUse, _fieldInfo.Name, _value });
                                  }
                                  else
                                  {
                                      // For CategoryBaseSingle<T>: SetField<U>(int index, string field_name, U value)
                                      genericMethod.Invoke(_category, new object[] { _itemIndex, _fieldInfo.Name, _value });
                                  }
                             }
                         }
                         catch (Exception ex)
                         {
                             Debug.WriteLine($"Error setting field {_fieldInfo.Name}: {ex.Message}");
                         }
                     }
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public string Type
        {
            get => _fieldInfo?.FieldType?.Name ?? "Unknown";
        }

        public EditableProperty(string propertyName, object value, FieldInfo? fieldInfo, SFEngine.SFCFF.ICategory? category, int itemIndex, int subIndex = -1)
        {
            PropertyName = propertyName;
            _value = value;
            _fieldInfo = fieldInfo;
            _category = category;
            _itemIndex = itemIndex;
            _subIndex = subIndex;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Element display system similar to WinForms SFControl
    public class ElementDisplayBase
    {
        protected SFEngine.SFCFF.ICategory category;

        public ElementDisplayBase(SFEngine.SFCFF.ICategory cat)
        {
            category = cat;
        }

        public virtual string GetElementString(int index)
        {
            try
            {
                category.GetID(index, out int elementId);

                // Try to get the item using reflection
                var itemsProperty = category.GetType().GetProperty("Items");
                if (itemsProperty != null)
                {
                    var items = itemsProperty.GetValue(category) as System.Collections.IList;
                    if (items != null && index < items.Count)
                    {
                        object item = items[index];
                        if (item != null)
                        {
                            var itemType = item.GetType();

                            // Try common name fields
                            var nameIdField = itemType.GetField("NameID") ?? itemType.GetField("TextID");
                            if (nameIdField != null)
                            {
                                var nameIdValue = nameIdField.GetValue(item);
                                if (nameIdValue != null && SFCategoryManager.gamedata?.c2054 != null)
                                {
                                    if (SFCategoryManager.gamedata.c2054.GetItemIndex((ushort)(int)nameIdValue, out int textIndex))
                                    {
                                        string text = SFCategoryManager.GetTextByLanguage(
                                            SFCategoryManager.gamedata.c2054[textIndex].TextID,
                                            SFEngine.Settings.LanguageID);
                                        return $"{elementId}: {text}";
                                    }
                                }
                            }

                            // For categories with direct text fields
                            var textField = itemType.GetField("Text") ?? itemType.GetField("Name");
                            if (textField != null)
                            {
                                var textValue = textField.GetValue(item);
                                if (textValue != null && !string.IsNullOrEmpty(textValue.ToString()))
                                {
                                    return $"{elementId}: {textValue}";
                                }
                            }
                        }
                    }
                }

                // Fallback to just ID
                return elementId.ToString();
            }
            catch
            {
                category.GetID(index, out int elementId);
                return elementId.ToString();
            }
        }
    }

    // Specific implementations for different categories
    public class SpellElementDisplay : ElementDisplayBase
    {
        public SpellElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                // Cast to the specific spell category type
                var spellCategory = category as Category2002;
                if (spellCategory != null && index < spellCategory.Items.Count)
                {
                    var spell = spellCategory.Items[index];
                    int spellId = spell.SpellID;
                    int spellLevel = spell.GetSpellLevel();

                    // Try to get spell name from text category (2054)
                    string spellName = $"Spell {spellId}";
                    if (SFCategoryManager.gamedata?.c2054 != null)
                    {
                        int spellLineId = spell.SpellLineID;
                        if (SFCategoryManager.gamedata.c2054.GetItemIndex((ushort)spellLineId, out int textIndex))
                        {
                            spellName = SFCategoryManager.GetTextByLanguage(
                                SFCategoryManager.gamedata.c2054[textIndex].TextID,
                                SFEngine.Settings.LanguageID);
                        }
                    }

                    return $"{spellId} {spellName} level {spellLevel}";
                }
            }
            catch
            {
                // Fall back to base implementation
            }

            return base.GetElementString(index);
        }
    }

    public class TextElementDisplay : ElementDisplayBase
    {
        public TextElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                var textCategory = category as Category2054;
                if (textCategory != null && index < textCategory.Items.Count)
                {
                    var text = textCategory.Items[index];
                    string displayText = SFCategoryManager.GetTextByLanguage(text.TextID, SFEngine.Settings.LanguageID);
                    return $"{text.TextID}: {displayText}";
                }
            }
            catch
            {
                // Fall back to base implementation
            }

            return base.GetElementString(index);
        }
    }

    public class ItemElementDisplay : ElementDisplayBase
    {
        public ItemElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                var itemCategory = category as Category2003;
                if (itemCategory != null && index < itemCategory.Items.Count)
                {
                    var item = itemCategory.Items[index];
                    string itemName = $"Item {item.ItemID}";
                    // Try to get item name from text
                    if (SFCategoryManager.gamedata?.c2054 != null)
                    {
                        if (SFCategoryManager.gamedata.c2054.GetItemIndex(item.NameID, out int textIndex))
                        {
                            itemName = SFCategoryManager.GetTextByLanguage(
                                SFCategoryManager.gamedata.c2054[textIndex].TextID,
                                SFEngine.Settings.LanguageID);
                        }
                    }
                    return $"{item.ItemID}: {itemName}";
                }
            }
            catch
            {
                // Fall back to base implementation
            }

            return base.GetElementString(index);
        }
    }

    public class BuildingElementDisplay : ElementDisplayBase
    {
        public BuildingElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                var buildingCategory = category as Category2029;
                if (buildingCategory != null && index < buildingCategory.Items.Count)
                {
                    var building = buildingCategory.Items[index];
                    string buildingName = $"Building {building.BuildingID}";
                    // Try to get building name from text
                    if (SFCategoryManager.gamedata?.c2054 != null)
                    {
                        if (SFCategoryManager.gamedata.c2054.GetItemIndex(building.NameID, out int textIndex))
                        {
                            buildingName = SFCategoryManager.GetTextByLanguage(
                                SFCategoryManager.gamedata.c2054[textIndex].TextID,
                                SFEngine.Settings.LanguageID);
                        }
                    }
                    return $"{building.BuildingID} {buildingName}";
                }
            }
            catch
            {
                // Fall back to base implementation
            }

            return base.GetElementString(index);
        }
    }

    public class RaceElementDisplay : ElementDisplayBase
    {
        public RaceElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                var raceCategory = category as Category2022;
                if (raceCategory != null && index < raceCategory.Items.Count)
                {
                    var race = raceCategory.Items[index];
                    string raceName = $"Race {race.RaceID}";
                    // Try to get race name from text
                    if (SFCategoryManager.gamedata?.c2054 != null)
                    {
                        if (SFCategoryManager.gamedata.c2054.GetItemIndex(race.TextID, out int textIndex))
                        {
                            raceName = SFCategoryManager.GetTextByLanguage(
                                SFCategoryManager.gamedata.c2054[textIndex].TextID,
                                SFEngine.Settings.LanguageID);
                        }
                    }
                    return $"{race.RaceID} {raceName}";
                }
            }
            catch
            {
                // Fall back to base implementation
            }

            return base.GetElementString(index);
        }
    }

    public class UnitElementDisplay : ElementDisplayBase
    {
        public UnitElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                var unitCategory = category as Category2025;
                if (unitCategory != null)
                {
                    // Category2025 is a sub-item category, need to handle differently
                    unitCategory.GetItemSubItemIndex(index, 0, out int subIndex);
                    if (subIndex >= 0)
                    {
                        var unit = unitCategory[subIndex];
                        return $"{unit.UnitID} {SFCategoryManager.GetUnitName(unit.UnitID)}";
                    }
                }
            }
            catch
            {
                // Fall back to base implementation
            }

            return base.GetElementString(index);
        }
    }

    public class HeroSkillsElementDisplay : ElementDisplayBase
    {
        public HeroSkillsElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                var skillsCategory = category as Category2006;
                if (skillsCategory != null)
                {
                    skillsCategory.GetID(index, out int statsId);
                    if (SFCategoryManager.gamedata?.c2005 != null &&
                        SFCategoryManager.gamedata.c2005.GetItemIndex(statsId, out int statsIndex))
                    {
                        var stats = SFCategoryManager.gamedata.c2005[statsIndex];
                        if (SFCategoryManager.hero_cache.GetItemIndex(stats.StatsID, out int heroIndex))
                        {
                            return $"{stats.StatsID} {SFCategoryManager.GetRuneheroName(stats.StatsID)} (lvl {stats.UnitLevel})";
                        }
                        else
                        {
                            return $"{stats.StatsID} {SFEngine.Utility.S_ITEM_MISSING}";
                        }
                    }
                }
            }
            catch
            {
                // Fall back to base implementation
            }

            return base.GetElementString(index);
        }
    }

    public class ItemArmorElementDisplay : ElementDisplayBase
    {
        public ItemArmorElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                category.GetID(index, out int id);
                return $"{id} {SFCategoryManager.GetItemName((ushort)id)}";
            }
            catch
            {
                return base.GetElementString(index);
            }
        }
    }

    public class SpellScrollElementDisplay : ElementDisplayBase
    {
        public SpellScrollElementDisplay(SFEngine.SFCFF.ICategory cat) : base(cat) { }

        public override string GetElementString(int index)
        {
            try
            {
                var scrollCategory = category as Category2013;
                if (scrollCategory != null && index < scrollCategory.Items.Count)
                {
                    var scroll = scrollCategory.Items[index];
                    string itemName = SFCategoryManager.GetItemName(scroll.ItemID);
                    string scrollName = SFCategoryManager.GetItemName(scroll.InstalledScrollItemID);
                    return $"{itemName} | {scrollName}";
                }
            }
            catch
            {
                // Fall back to base implementation
            }

            return base.GetElementString(index);
        }
    }

    public partial class CffEditorWindow : Window
    {
        private ComboBox categorySelect;
        private ListBox elementSelect;
        private DataGrid itemDataGrid;
        private Button saveFileButton;
        private TextBox searchBox;
        private TextBox elementSearchBox;
        private SFEngine.SFCFF.SFGameDataNew gameData;
        private string currentFilePath;
        private Dictionary<int, ElementDisplayBase> cachedElementDisplays = new Dictionary<int, ElementDisplayBase>();

        public CffEditorWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        public CffEditorWindow(SFEngine.SFCFF.SFGameDataNew initialGameData, string filePath = null)
        {
            InitializeComponent();
            gameData = initialGameData;
            currentFilePath = filePath;

            // Set the global game data reference so SFCategoryManager works
            SFEngine.SFCFF.SFCategoryManager.gamedata = gameData;

            InitializeControls();
            InitializeElementDisplays();
            PopulateCategories();
        }

        private void InitializeElementDisplays()
        {
            if (gameData == null) return;

            foreach (var category in gameData.GetCategories())
            {
                int categoryId = category.GetCategoryID();
                cachedElementDisplays[categoryId] = GetElementDisplayForCategory(categoryId, category);
            }
        }

        private void InitializeControls()
        {
            categorySelect = this.FindControl<ComboBox>("CategorySelect");
            if (categorySelect != null)
            {
                categorySelect.SelectionChanged += CategorySelect_SelectionChanged;
            }
            elementSelect = this.FindControl<ListBox>("ElementSelect");
            if (elementSelect != null)
            {
                elementSelect.SelectionChanged += ElementSelect_SelectionChanged;
            }
            itemDataGrid = this.FindControl<DataGrid>("ItemDataGrid");
            saveFileButton = this.FindControl<Button>("SaveFileButton");
            if (saveFileButton != null)
            {
                saveFileButton.Click += SaveFileButton_Click;
            }
            searchBox = this.FindControl<TextBox>("SearchBox");
            if (searchBox != null)
            {
                searchBox.TextChanged += SearchBox_TextChanged;
            }
            elementSearchBox = this.FindControl<TextBox>("ElementSearchBox");
            if (elementSearchBox != null)
            {
                elementSearchBox.TextChanged += ElementSearchBox_TextChanged;
            }
        }

        private List<string> allCategoryNames = new List<string>();
        private List<int> filteredElementIds = new List<int>();

        private ElementDisplayBase GetElementDisplayForCategory(int categoryId, SFEngine.SFCFF.ICategory category)
        {
            if (cachedElementDisplays.ContainsKey(categoryId))
            {
                return cachedElementDisplays[categoryId];
            }

            ElementDisplayBase display;
            switch (categoryId)
            {
                case 2002: // Spells
                    display = new SpellElementDisplay(category);
                    break;
                case 2054: // Text
                    display = new TextElementDisplay(category);
                    break;
                case 2003: // Items
                    display = new ItemElementDisplay(category);
                    break;
                case 2029: // Buildings
                    display = new BuildingElementDisplay(category);
                    break;
                case 2022: // Races
                    display = new RaceElementDisplay(category);
                    break;
                case 2025: // Units
                    display = new UnitElementDisplay(category);
                    break;
                case 2006: // Hero/worker skills
                    display = new HeroSkillsElementDisplay(category);
                    break;
                case 2004: // Item armor data
                    display = new ItemArmorElementDisplay(category);
                    break;
                case 2013: // Inventory spell scroll link
                    display = new SpellScrollElementDisplay(category);
                    break;
                // Add more cases as needed
                default:
                    display = new ElementDisplayBase(category);
                    break;
            }

            cachedElementDisplays[categoryId] = display;
            return display;
        }

        private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (gameData == null || textBox == null || string.IsNullOrEmpty(textBox.Text))
            {
                PopulateCategories();
                return;
            }

            var searchTerm = textBox.Text.ToLowerInvariant();
            var filteredCategories = gameData.GetCategories()
                .Where(cat => cat.GetName().ToLowerInvariant().IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            categorySelect.Items.Clear();
            allCategoryNames.Clear();
            foreach (var category in filteredCategories)
            {
                categorySelect.Items.Add(category.GetName());
                allCategoryNames.Add(category.GetName());
            }
        }

        private void ElementSearchBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (categorySelect == null || elementSelect == null || textBox == null || string.IsNullOrEmpty(textBox.Text))
            {
                PopulateElements();
                return;
            }

            var searchTerm = textBox.Text.ToLowerInvariant();
            var selectedCategory = gameData.GetCategories().ToList()[categorySelect.SelectedIndex];

            filteredElementIds.Clear();
            for (int i = 0; i < selectedCategory.GetNumOfItems(); i++)
            {
                var elementString = get_element_string(selectedCategory, i);
                if (elementString.ToLowerInvariant().IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filteredElementIds.Add(i); // Store the index instead of ID
                }
            }

            elementSelect.Items.Clear();
            foreach (var index in filteredElementIds)
            {
                elementSelect.Items.Add(get_element_string(selectedCategory, index));
            }
        }

        private void PopulateCategories()
        {
            if (gameData == null)
            {
                return;
            }

            var items = new List<string>();
            foreach (var category in gameData.GetCategories())
            {
                items.Add(category.GetName());
            }
            categorySelect.Items.Clear();
            foreach (var item in items)
            {
                categorySelect.Items.Add(item);
            }
        }

        private void PopulateElements()
        {
            if (categorySelect == null || categorySelect.SelectedItem == null || gameData == null)
            {
                return;
            }

            if (categorySelect.SelectedIndex < 0 || categorySelect.SelectedIndex >= gameData.GetCategories().Count())
            {
                return;
            }

            var selectedCategory = gameData.GetCategories().ToList()[categorySelect.SelectedIndex];
            var items = new List<string>();
            for (int i = 0; i < selectedCategory.GetNumOfItems(); i++)
            {
                items.Add(get_element_string(selectedCategory, i));
            }
            elementSelect.Items.Clear();
            foreach (var item in items)
            {
                elementSelect.Items.Add(item);
            }
        }

        private string get_element_string(SFEngine.SFCFF.ICategory category, int index)
        {
            try
            {
                int categoryId = category.GetCategoryID();
                if (cachedElementDisplays.ContainsKey(categoryId))
                {
                    return cachedElementDisplays[categoryId].GetElementString(index);
                }
                else
                {
                    // Fallback if no display is cached
                    category.GetID(index, out int elementId);
                    return elementId.ToString();
                }
            }
            catch
            {
                // If anything fails, just return the ID
                category.GetID(index, out int elementId);
                return elementId.ToString();
            }
        }

        private void CategorySelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categorySelect != null && categorySelect.SelectedItem != null)
            {
                PopulateElements();
            }
            if (itemDataGrid != null)
            {
                itemDataGrid.ItemsSource = null; // Clear data grid when category changes
            }
        }

        private void ElementSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (elementSelect == null || elementSelect.SelectedItem == null ||
                categorySelect == null || categorySelect.SelectedItem == null ||
                gameData == null || itemDataGrid == null)
            {
                if (itemDataGrid != null)
                {
                    itemDataGrid.ItemsSource = null;
                }
                return;
            }

            if (categorySelect.SelectedIndex < 0 || categorySelect.SelectedIndex >= gameData.GetCategories().Count())
            {
                itemDataGrid.ItemsSource = null;
                return;
            }

            var selectedCategory = gameData.GetCategories().ToList()[categorySelect.SelectedIndex];

            // Find the actual element index (not the display index)
            // Since we're filtering, we need to map back to the original index
            int selectedIndex = -1;

            if (elementSearchBox != null && !string.IsNullOrEmpty(elementSearchBox.Text))
            {
                // If we're filtering, use the filtered index mapping
                int displayIndex = elementSelect.SelectedIndex;
                if (displayIndex >= 0 && displayIndex < filteredElementIds.Count)
                {
                    selectedIndex = filteredElementIds[displayIndex];
                }
            }
            else
            {
                // No filtering, direct index
                selectedIndex = elementSelect.SelectedIndex;
            }

            if (selectedIndex >= 0 && selectedIndex < selectedCategory.GetNumOfItems())
            {
                try
                {
                    // Get the element data using the category's specific accessor
                    // This depends on the category type, so we use reflection safely
                    var properties = new List<EditableProperty>();

                    // Check the category type
                    var categoryType = selectedCategory.GetType();
                    Debug.WriteLine($"Category type: {categoryType.Name}, Category ID: {selectedCategory.GetCategoryID()}");

                    // Debug: Show all properties on the category
                    var allProperties = categoryType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    Debug.WriteLine($"Category has {allProperties.Length} properties:");
                    foreach (var prop in allProperties)
                    {
                        Debug.WriteLine($"  Property: {prop.Name} ({prop.PropertyType.Name})");
                    }

                    // Try to get the element using reflection
                    // Different category types have different access patterns
                     try
                     {
                         // Handle different category base types
                         var itemsProperty = categoryType.GetProperty("Items");
                         if (itemsProperty != null)
                         {
                             var items = itemsProperty.GetValue(selectedCategory) as System.Collections.IList;
                             if (items != null)
                             {
                                 int itemIndex = selectedIndex;

                                 // For CategoryBaseMultiple<T>, selectedIndex is the main item index, need to get sub-item index
                                 if (categoryType.BaseType != null && categoryType.BaseType.IsGenericType &&
                                     categoryType.BaseType.GetGenericTypeDefinition() == typeof(SFEngine.SFCFF.CategoryBaseMultiple<>))
                                 {
                                     Debug.WriteLine("Detected CategoryBaseMultiple<T>, getting sub-item index");
                                     var getSubItemIndexMethod = categoryType.GetMethod("GetSubItemIndex");
                                     if (getSubItemIndexMethod != null)
                                     {
                                         itemIndex = (int)getSubItemIndexMethod.Invoke(selectedCategory, new object[] { selectedIndex, 0 });
                                         Debug.WriteLine($"Main item index {selectedIndex} -> sub-item index {itemIndex}");
                                     }
                                 }

                                 if (itemIndex >= 0 && itemIndex < items.Count)
                                 {
                                     object item = items[itemIndex];
                                     if (item != null)
                                     {
                                         Debug.WriteLine($"Found item of type {item.GetType().Name} at index {itemIndex}");
                                         foreach (var field in item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                                         {
                                             var value = field.GetValue(item);
                                              // For CategoryBaseMultiple<T>, pass the main index and sub-index; for others, pass the item index
                                              if (categoryType.BaseType != null && categoryType.BaseType.IsGenericType &&
                                                  categoryType.BaseType.GetGenericTypeDefinition() == typeof(SFEngine.SFCFF.CategoryBaseMultiple<>))
                                              {
                                                  properties.Add(new EditableProperty(field.Name, value, field, selectedCategory, selectedIndex, 0));
                                              }
                                              else
                                              {
                                                  properties.Add(new EditableProperty(field.Name, value, field, selectedCategory, itemIndex));
                                              }
                                         }
                                     }
                                 }
                                 else
                                 {
                                     Debug.WriteLine($"Item index {itemIndex} out of range (0-{items.Count-1})");
                                 }
                             }
                             else
                             {
                                 Debug.WriteLine("Items property is null");
                             }
                         }
                         else
                         {
                             Debug.WriteLine($"No Items property found for category {selectedCategory.GetCategoryID()}");
                             // Try indexer access
                             var indexer = categoryType.GetProperty("Item");
                             if (indexer != null)
                             {
                                 Debug.WriteLine("Trying indexer access...");
                                 object[] indexerArgs;

                                 // For CategoryBaseMultiple<T>, use two-parameter indexer
                                 if (categoryType.BaseType != null && categoryType.BaseType.IsGenericType &&
                                     categoryType.BaseType.GetGenericTypeDefinition() == typeof(SFEngine.SFCFF.CategoryBaseMultiple<>))
                                 {
                                     indexerArgs = new object[] { selectedIndex, 0 };
                                     Debug.WriteLine("Using two-parameter indexer for CategoryBaseMultiple<T>");
                                 }
                                 else
                                 {
                                     indexerArgs = new object[] { selectedIndex };
                                 }

                                 var item = indexer.GetValue(selectedCategory, indexerArgs);
                                 if (item != null)
                                 {
                                     Debug.WriteLine($"Indexer returned item of type {item.GetType().Name}");
                                     int propertyIndex = selectedIndex;

                                      foreach (var field in item.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                                      {
                                          var value = field.GetValue(item);
                                          // For CategoryBaseMultiple<T>, pass the main index and sub-index; for others, pass the selected index
                                          if (categoryType.BaseType != null && categoryType.BaseType.IsGenericType &&
                                              categoryType.BaseType.GetGenericTypeDefinition() == typeof(SFEngine.SFCFF.CategoryBaseMultiple<>))
                                          {
                                              properties.Add(new EditableProperty(field.Name, value, field, selectedCategory, selectedIndex, 0));
                                          }
                                          else
                                          {
                                              properties.Add(new EditableProperty(field.Name, value, field, selectedCategory, selectedIndex));
                                          }
                                      }
                                 }
                                 else
                                 {
                                     Debug.WriteLine("Indexer returned null");
                                 }
                             }
                             else
                             {
                                 Debug.WriteLine("No indexer found either");
                             }
                         }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error accessing category data: {ex.Message}");
                        Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }

                    Debug.WriteLine($"Setting {properties.Count} properties for element at index {selectedIndex}");
                    foreach (var prop in properties)
                    {
                        Debug.WriteLine($"Property: {prop.PropertyName} = {prop.Value} (Type: {prop.Type})");
                    }
                    itemDataGrid.ItemsSource = properties;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading element properties: {ex.Message}");
                    Debug.WriteLine($"Category: {selectedCategory.GetCategoryID()}, Index: {selectedIndex}, NumItems: {selectedCategory.GetNumOfItems()}");
                    itemDataGrid.ItemsSource = null;
                }
            }
            else
            {
                Debug.WriteLine($"Invalid selectedIndex: {selectedIndex}, NumItems: {selectedCategory.GetNumOfItems()}");
                itemDataGrid.ItemsSource = null;
            }
        }

        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameData == null)
            {
                Debug.WriteLine("Error: No data loaded to save.");
                return;
            }

            try
            {
                string savePath = currentFilePath;
                
                // If no current file path, show save dialog
                if (string.IsNullOrEmpty(savePath))
                {
                    var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                    {
                        Title = "Save CFF File",
                        DefaultExtension = "cff",
                        FileTypeChoices = new[]
                        {
                            new FilePickerFileType("CFF files")
                            {
                                Patterns = new[] { "*.cff" },
                                AppleUniformTypeIdentifiers = new[] { "public.data" }
                            }
                        },
                        SuggestedFileName = "GameData_new.cff"
                    });

                    if (file != null)
                    {
                        savePath = file.Path.LocalPath;
                    }
                    else
                    {
                        return; // User cancelled
                    }
                }

                // Save the data
                gameData.Save(savePath);
                currentFilePath = savePath;
                
                Debug.WriteLine($"File saved successfully to: {savePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save file: {ex.Message}");
            }
        }
    }
}
