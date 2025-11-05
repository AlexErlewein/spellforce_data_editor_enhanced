using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SFEngine.SFCFF;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace SpellforceDataEditor.Avalonia
{
    public partial class CffEditorWindow : Window
    {
        private ComboBox categorySelect;
        private ListBox elementSelect;
        private DataGrid itemDataGrid;
        private Button saveFileButton;
        private SFEngine.SFCFF.SFGameDataNew gameData;

        public CffEditorWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        public CffEditorWindow(SFEngine.SFCFF.SFGameDataNew initialGameData)
        {
            InitializeComponent();
            gameData = initialGameData;
            InitializeControls();
            PopulateCategories();
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

        private void CategorySelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categorySelect.SelectedItem == null)
            {
                return;
            }

            var selectedCategory = gameData.GetCategories().ToList()[categorySelect.SelectedIndex];
            var items = new List<string>();
            for (int i = 0; i < selectedCategory.GetNumOfItems(); i++)
            {
                selectedCategory.GetID(i, out int id);
                items.Add(id.ToString());
            }
            elementSelect.Items.Clear();
            foreach (var item in items)
            {
                elementSelect.Items.Add(item);
            }
            itemDataGrid.ItemsSource = null; // Clear data grid when category changes
        }

        private void ElementSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (elementSelect.SelectedItem == null || categorySelect.SelectedItem == null)
            {
                itemDataGrid.ItemsSource = null;
                return;
            }

            var selectedCategory = gameData.GetCategories().ToList()[categorySelect.SelectedIndex];
            if (int.TryParse(elementSelect.SelectedItem.ToString(), out int selectedId))
            {
                if (selectedCategory.GetItemIndex(selectedId, out int itemIndex))
                {
                    // Using reflection to get item properties
                    object item = ((dynamic)selectedCategory).Items[itemIndex]; // Cast to object to avoid dynamic lambda issue
                    var properties = item.GetType().GetFields().Select(f => new { Property = f.Name, Value = f.GetValue(item) }).ToList();
                    itemDataGrid.ItemsSource = properties;
                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for save logic
            // For now, just show a message box
            Debug.WriteLine("Save functionality not yet implemented.");
        }
    }
}
